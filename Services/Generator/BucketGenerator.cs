using Newtonsoft.Json;
using NFTGenApi.Models;
using System.Security.Cryptography;
using static NFTGenApi.Services.Helpers;

namespace NFTGenApi.Services.Generator;

public class Buckets
{
    public Bucket NumberOfLayers { get; set; } = new Bucket();
    public Bucket Layers { get; set; } = new Bucket();
    public List<Bucket> Traits { get; set; } = new List<Bucket>();

    public Buckets Copy()
    {
        var serialised = JsonConvert.SerializeObject(this);
        return JsonConvert.DeserializeObject<Buckets>(serialised) ?? this;
    }
}

public class Bucket
{
    public string Name { get; set; } = string.Empty;
    public TARGET_TYPE Type { get; set; }
    public List<BucketItem> BucketList { get; set; } = new List<BucketItem>();
    public BucketItem RollBucketListV2()
    {
        int seed = (int)DateTime.Now.Ticks & 0x0000FFFF; // Using lower 16 bits of the tick count
        Random rng = new Random(seed);
        List<BucketItem> _bucketList = BucketList.OrderByDescending(x => x.Value).ToList();

        List<string> selectionList = new List<string>(); // max 100
        selectionList.Capacity = 100;

        foreach (var bucket in _bucketList)
        {
            selectionList.AddRange(Enumerable.Repeat(bucket.Name, AmountToAdd(selectionList, bucket)).ToList());
        }

        var selected = GetRandomIntInRange(rng, selectionList.Count);
        var outcome = _bucketList.Where(b => b.Name == selectionList[selected]).First();
        BucketList.Remove(outcome);

        // balance from removal
        BucketList.ForEach(b =>
        {
            var outcomeValue = outcome.Value == 1 ? 0 : outcome.Value;
            b.Value = b.Value / (1 - outcomeValue);
        });

        return outcome;
    }

    public BucketItem RollBucketListV3(Outcome state, Matrix matrix)
    {
        List<BucketItem> _bucketList = BucketList.ToList();

        foreach (var item in _bucketList)
        {
            foreach (var outcomeItem in state.GetAll)
            {
                var x_value = matrix.GetValue(item.Name, item.Type, outcomeItem.Name, outcomeItem.Type);
                if (item == outcomeItem) // this will maybe happen when we are picking layer type
                {
                    x_value = -1;
                }
                //Console.WriteLine($"{item.Name}\t{outcomeItem.Name}\t{x_value}");
                item.Value = x_value == 0 ? item.Value : x_value;
            }
        }

        PrintValues(_bucketList);

        var selectedItem = BalanceAndSelectItem(_bucketList);
        Console.WriteLine($"Selected item: {selectedItem.Type}\t{selectedItem.Name}\t{selectedItem.Value}");
        return selectedItem;
    }

    private void PrintValues(List<BucketItem> b)
    {
        Console.WriteLine("\n---\n");
        foreach (var i in b)
        {
            Console.WriteLine($"{i.Type}\t{i.Name}\t{i.Value}");
        }
        Console.WriteLine("\n---\n");
    }

    private BucketItem BalanceAndSelectItem(List<BucketItem> _bucketList)
    {
        _bucketList.RemoveAll(b => b.Value == -1); // remove any item that has -1

        // if we have any 1's
        if (_bucketList.Where(b => b.Value == 1).Any())
        {
            _bucketList.RemoveAll(b => b.Value != 1); // if we;re here it means we have a MUST constraint
        }

        Rebalance(_bucketList);

        return SelectItem(_bucketList);
    }

    private BucketItem SelectItem(List<BucketItem> _bucketList)
    {
        Random rng = new Random();
        double randomValue = rng.NextDouble();
        _bucketList = _bucketList.OrderBy(i => rng.Next()).ToList();

        double cumSum = 0;
        foreach (var item in _bucketList)
        {
            cumSum += (double)item.Value;
            if (cumSum >= randomValue)
            {
                return item;
            }
        }

        //fallback
        return _bucketList.Last();
    }

    private void Rebalance(List<BucketItem> _bucketList)
    {
        /*
         * == 1 - return that
         * >1 do we want to then use it as a weight between all of those that have 1's?
         * == -1 remove from list
         * else
         * treat all as weights?
         * rebalance all as a %?
        */

        var sum = _bucketList.Sum(b => b.Value);
        foreach (var item in _bucketList)
        {
            item.Value = item.Value / sum;
        }
    }

    public static int GetRandomIntInRange(Random rng, int length)
    {
        Dictionary<int, int> randNumGen = new Dictionary<int, int>();
        for (var i = 0; i < length; i++)
        {
            var r = rng.Next(length);
            if (randNumGen.ContainsKey(r))
            {
                randNumGen[r] += 1;
            }
            else
            {
                randNumGen[r] = 1;
            }
        }
        var mostFreq = randNumGen.OrderByDescending(k => k.Value).First().Key;
        return mostFreq;
    }

    private int AmountToAdd(List<string> selectionList, BucketItem bucket)
    {
        var usedCapacity = selectionList.Count;
        var availCapacity = selectionList.Capacity - usedCapacity;
        var wantingToAdd = (int)(bucket.Value * 100);
        return wantingToAdd > availCapacity ? availCapacity : wantingToAdd;
    }

}

public class BucketItem
{
    public string Name { get; set; } = string.Empty;
    public TARGET_TYPE Type { get; set; }
    public decimal Value { get; set; } = 0M;
}

public static class BucketGenerator
{
    public static Buckets CreateBuckets(Properties properties)
    {
        // create bucket for number of layer changes
        Buckets buckets = new Buckets();
        buckets.NumberOfLayers = CreateLayerCountBucket(properties);
        buckets.Layers = CreateLayerBucket(properties.Layers);

        foreach (var layer in properties.Layers)
        {
            buckets.Traits.Add(CreateTraitBucket(layer));
        }

        return buckets;
    }

    private static Bucket CreateLayerCountBucket(Properties properties)
    {
        HashSet<LayersProperty> layerProperties = properties.LayersProperties;
        List<Layer> layers = properties.Layers;

        Bucket buckets = new Bucket()
        {
            Name = TARGET_TYPE.LAYERCOUNT.ToString(),
            Type = TARGET_TYPE.LAYERCOUNT,
        };

        // find all layers that have alwaysOn
        var minLayers = layers.Count(l => l.Frequency.AlwaysOn);

        foreach (var element in layerProperties)
        {
            decimal value = element.Frequency.Value;
            if (minLayers > element.AmountOfLayers)
            {
                value = 0;
            }
            buckets.BucketList.Add(new BucketItem()
            {
                Name = element.AmountOfLayers.ToString(),
                Type = TARGET_TYPE.LAYERCOUNT,
                Value = IntToDecimal(value),
            });
        }
        return buckets;
    }

    private static Bucket CreateLayerBucket(List<Layer> layers)
    {
        Bucket buckets = new Bucket()
        {
            Name = TARGET_TYPE.LAYER.ToString(),
            Type = TARGET_TYPE.LAYER,
        };

        foreach (var layer in layers)
        {
            buckets.BucketList.Add(new BucketItem()
            {
                Name = layer.Name,
                Type = TARGET_TYPE.LAYER,
                Value = IntToDecimal(layer.Frequency.Value),
            });
        }
        return buckets;
    }

    private static Bucket CreateTraitBucket(Layer layer)
    {
        Bucket buckets = new Bucket()
        {
            Name = layer.Name,
            Type = TARGET_TYPE.TRAIT,
        };

        foreach (var trait in layer.Traits)
        {
            buckets.BucketList.Add(new BucketItem()
            {
                Name = trait.Name,
                Type = TARGET_TYPE.TRAIT,
                Value = IntToDecimal(trait.Frequency.Value),
            });
        }
        return buckets;
    }

}
