using Microsoft.AspNetCore.Mvc.Formatters;
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
    public string Name { get; set; } = string.Empty;
    public CONTAINER_TYPE Type { get; set; }
    public List<Bucket> BucketList { get; set; } = new List<Bucket>();

    public Bucket RollBucketList()
    {
        Random rng = new Random();
        //List<Bucket> _bucketList = BucketList.OrderBy(x => x.Value).ToList();
        List<Bucket> _bucketList = BucketList.OrderByDescending(x => x.Value).ToList();

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

    public Bucket RollBucketListV2()
    {
        Random rng = new Random();
        List<Bucket> _bucketList = BucketList.OrderByDescending(x => x.Value).ToList();

        List<string> selectionList = new List<string>(); // max 100
        selectionList.Capacity = 100;

        foreach (var bucket in _bucketList)
        {
            selectionList.AddRange(Enumerable.Repeat(bucket.Name, AmountToAdd(selectionList, bucket)).ToList());
        }

        Dictionary<int, int> randNumGen = new Dictionary<int, int>();
        for (var i = 0; i < selectionList.Count; i++)
        {
            var r = rng.Next(100);
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
        var outcome = _bucketList.Where(b => b.Name == selectionList[mostFreq]).First();
        BucketList.Remove(outcome);

        return outcome;

    }

    private int AmountToAdd(List<string> selectionList, Bucket bucket)
    {
        var usedCapacity = selectionList.Count;
        var availCapacity = selectionList.Capacity - usedCapacity;
        var wantingToAdd = (int)(bucket.Value * 100);
        return wantingToAdd > availCapacity ? availCapacity : wantingToAdd;
    }

}

public class Bucket
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; } = 0M;
}

public static class BucketGenerator
{
    public static List<Buckets> CreateBuckets(Properties properties)
    {
        // FOR NOW ASSUME THAT ALL FREQUENCIES ARE IN A PERCENTAGE
        // WE WILL DO THE CONVERSION HIGHER


        // create bucket for number of layer changes
        List<Buckets> buckets = new List<Buckets>
        {
            CreateLayerCountBucket(properties.LayersProperties),
            CreateLayerBucket(properties.Layers)
        };

        foreach (var layer in properties.Layers)
        {
            buckets.Add(CreateTraitBucket(layer));
        }


        return buckets;
    }

    private static Buckets CreateLayerCountBucket(HashSet<LayersProperty> layerProperties)
    {
        Buckets buckets = new Buckets()
        {
            Name = CONTAINER_TYPE.LAYERCOUNT.ToString(),
            Type = CONTAINER_TYPE.LAYERCOUNT,
        };

        foreach (var element in layerProperties)
        {
            buckets.BucketList.Add(new Bucket()
            {
                Name = element.AmountOfLayers.ToString(),
                Value = IntToDecimal(element.Frequency.Value),
            });
        }
        return buckets;
    }

    private static Buckets CreateLayerBucket(List<Layer> layers)
    {
        Buckets buckets = new Buckets()
        {
            Name = CONTAINER_TYPE.LAYER.ToString(),
            Type = CONTAINER_TYPE.LAYER,
        };

        foreach (var layer in layers)
        {
            buckets.BucketList.Add(new Bucket()
            {
                Name = layer.Name,
                Value = IntToDecimal(layer.Frequency.Value),
            });
        }
        return buckets;
    }
    private static Buckets CreateTraitBucket(Layer layer)
    {
        Buckets buckets = new Buckets()
        {
            Name = layer.Name,
            Type = CONTAINER_TYPE.TRAIT,
        };

        foreach (var trait in layer.Traits)
        {
            buckets.BucketList.Add(new Bucket()
            {
                Name = trait.Name,
                Value = IntToDecimal(trait.Frequency.Value),
            });
        }
        return buckets;
    }

}
