using Newtonsoft.Json;
using NFTGenApi.Models;
using static NFTGenApi.Services.Helpers.Helpers;

namespace NFTGenApi.Services.Generator;

public enum CONTAINER_TYPE
{
    LAYER = 0, // contains a list<bucket> of all layers
    TRAIT = 1, // contains a list<bucket> of traits
    LAYERCOUNT = 2, // contains a list<bucket> of layer count chances
}

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
    public CONTAINER_TYPE Type { get; set; }
    public List<BucketItem> BucketList { get; set; } = new List<BucketItem>();

    public Bucket Rebalance(List<BucketItem> bucketList, Outcome state, Matrix matrix)
    {
        // if matrix rule exists, set % to that.

        // if matrix rule exists, alter other
        return new Bucket();
    }
    public BucketItem RollBucketList(Outcome state, Matrix matrix)
    {
        Random rng = new Random();
        //List<Bucket> _bucketList = BucketList.OrderBy(x => x.Value).ToList();
        List<BucketItem> _bucketList = BucketList.OrderByDescending(x => x.Value).ToList();

        // get alterations from matrix
        // rebalance - this will happen regardless

        //decimal totalChance = 0M;
        int rolls = 0;

        while (true)
        {
            foreach (var bucket in _bucketList)
            {
                //totalChance += bucket.Value;
                rolls++; //rm
                if (new decimal(rng.NextDouble()) <= bucket.Value)
                {
                    var outcome = bucket;
                    BucketList.Remove(bucket);
                    Console.WriteLine(outcome.Name + " with a chance of " + outcome.Value + "% was selected after " + rolls + " rolls");
                    return outcome;
                }
            }
        }
    }

    public BucketItem RollBucketListV2()
    {
        Random rng = new Random();
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
    public decimal Value { get; set; } = 0M;
}

public static class BucketGenerator
{
    public static Buckets CreateBuckets(Properties properties)
    {
        // FOR NOW ASSUME THAT ALL FREQUENCIES ARE IN A PERCENTAGE
        // WE WILL DO THE CONVERSION HIGHER


        // create bucket for number of layer changes
        Buckets buckets = new Buckets();
        buckets.NumberOfLayers = CreateLayerCountBucket(properties.LayersProperties);
        buckets.Layers = CreateLayerBucket(properties.Layers);

        foreach (var layer in properties.Layers)
        {
            buckets.Traits.Add(CreateTraitBucket(layer));
        }

        return buckets;
    }

    private static Bucket CreateLayerCountBucket(HashSet<LayersProperty> layerProperties)
    {
        Bucket buckets = new Bucket()
        {
            Name = CONTAINER_TYPE.LAYERCOUNT.ToString(),
            Type = CONTAINER_TYPE.LAYERCOUNT,
        };

        foreach (var element in layerProperties)
        {
            buckets.BucketList.Add(new BucketItem()
            {
                Name = element.AmountOfLayers.ToString(),
                Value = IntToDecimal(element.Frequency.Value),
            });
        }
        return buckets;
    }

    private static Bucket CreateLayerBucket(List<Layer> layers)
    {
        Bucket buckets = new Bucket()
        {
            Name = CONTAINER_TYPE.LAYER.ToString(),
            Type = CONTAINER_TYPE.LAYER,
        };

        foreach (var layer in layers)
        {
            buckets.BucketList.Add(new BucketItem()
            {
                Name = layer.Name,
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
            Type = CONTAINER_TYPE.TRAIT,
        };

        foreach (var trait in layer.Traits)
        {
            buckets.BucketList.Add(new BucketItem()
            {
                Name = trait.Name,
                Value = IntToDecimal(trait.Frequency.Value),
            });
        }
        return buckets;
    }

}
