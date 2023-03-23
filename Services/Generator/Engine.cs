using NFTGenApi.Models;

namespace NFTGenApi.Services.Generator;

public static class Engine
{
    public static List<Outcome> Generate(Properties properties)
    {
        // TODO VALIDATION ON INPUT OBJECT


        // create matrix
        Matrix matrix = MatrixGenerator.CreateMatrix(properties);

        /*
         * create buckets for each roll type
         * start at how many traits to have.
        */
        Buckets buckets = BucketGenerator.CreateBuckets(properties);

        // create outcome object
        List<Outcome> outcome = new List<Outcome>();
        for (int i = 0; i < 10000; i++)
        {
            outcome.Add(GenerateOutcome(buckets));
        }

        return outcome.Take(2).ToList();
    }

    private static Outcome GenerateOutcome(Buckets buckets)
    {
        Outcome outcome = new Outcome();
        Buckets _buckets = buckets.Copy();

        outcome.NumberOfLayers = _buckets.NumberOfLayers.RollBucketListV2();

        // get layers
        for (int i = 0; i < int.Parse(outcome.NumberOfLayers.Name); i++)
        {
            outcome.Layers.Add(_buckets.Layers.RollBucketListV2());

            // check if valid against matrix
        }

        // get traits
        for (int i = 0; i < int.Parse(outcome.NumberOfLayers.Name); i++)
        {
            outcome.Traits.Add(_buckets.Traits.Where(b => b.Name == outcome.Layers[i].Name).First().RollBucketListV2());

            // check if valid against matrix
        }

        // check for uniqueness

        return outcome;
    }
}
public record Outcome
{
    public BucketItem NumberOfLayers { get; set; } = new BucketItem();
    public List<BucketItem> Layers { get; set; } = new List<BucketItem>();
    public List<BucketItem> Traits { get; set; } = new List<BucketItem>();
}
