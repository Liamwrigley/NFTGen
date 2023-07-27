using Newtonsoft.Json;
using NFTGenApi.Models;
using System.Runtime.CompilerServices;

namespace NFTGenApi.Services.Generator;

/*
 * Helpful % calculation chat - https://chat.openai.com/share/2c9adfdc-179d-441e-8f09-4c660d2a6e1e
 * 
 * 
 * 
 * 
 */

public static class Engine
{
    public static List<OutcomeModel> Generate(Properties properties)
    {
        // TODO VALIDATION ON INPUT OBJECT


        // create matrix
        Matrix matrix = MatrixGenerator.CreateMatrix(properties);

        /*
         * create buckets for each roll type
         * start at how many traits to have.
        */
        Buckets buckets = BucketGenerator.CreateBuckets(properties);

        // create all outcomes
        List<Outcome> outcomes = GenerateOutcomes(buckets, matrix, properties.NumberOfItems);

        return outcomes.Select(o => o.ToModel()).ToList();
    }


    // TODO:MOVE to outcome generator
    private static List<Outcome> GenerateOutcomes(Buckets buckets, Matrix matrix, int generateAmount)
    {
        List<Outcome> outcomes = new List<Outcome>();
        HashSet<string> existingPermeations = new HashSet<string>();

        for (int i = 0; i < generateAmount; i++)
        {
            Outcome outcome = GenerateOutcome(buckets, matrix, existingPermeations);
            outcome.Id = i + 1;
            outcomes.Add(outcome);
        }
        return outcomes;
    }

    private static Outcome GenerateOutcome(Buckets buckets, Matrix matrix, HashSet<string> existingPermeations, int attempt = 0, int attemptsAllowed = 1000)
    {
        if (attempt > attemptsAllowed)
        {
            Console.WriteLine($"{existingPermeations.Count}");
            throw new Exception("cant find outcome");
        }

        Outcome outcome = new Outcome();
        Buckets _buckets = buckets.Copy();

        outcome.NumberOfLayers = _buckets.NumberOfLayers.RollBucketListV3(outcome, matrix);
        //outcome.NumberOfLayers = _buckets.NumberOfLayers.RollBucketListV2();

        // get layers
        for (int i = 0; i < int.Parse(outcome.NumberOfLayers.Name); i++)
        {
            outcome.Layers.Add(_buckets.Layers.RollBucketListV3(outcome, matrix));
            //outcome.Layers.Add(_buckets.Layers.RollBucketListV2());

            // check if valid against matrix
        }

        // get traits
        for (int i = 0; i < int.Parse(outcome.NumberOfLayers.Name); i++)
        {
            outcome.Traits.Add(_buckets.Traits.Where(b => b.Name == outcome.Layers[i].Name).First().RollBucketListV3(outcome, matrix));
            //outcome.Traits.Add(_buckets.Traits.Where(b => b.Name == outcome.Layers[i].Name).First().RollBucketListV2());


            // check if valid against matrix
        }

        // check for uniqueness
        if (DuplicateCheck(outcome, existingPermeations))
        {
            Console.WriteLine($"Calling againt. Attempt: {attempt + 1}");
            outcome = GenerateOutcome(buckets, matrix, existingPermeations, attempt + 1);
        }
        return outcome;
    }

    private static bool DuplicateCheck(Outcome outcome, HashSet<string> existingPermeations)
    {
        string outcomeHash = Helpers.ComputeSHA256HashAsString(JsonConvert.SerializeObject(outcome));
        if (existingPermeations.Contains(outcomeHash))
        {
            return true;
        }
        existingPermeations.Add(outcomeHash);
        outcome.Hash = outcomeHash;
        return false;
    }
}
public class Outcome
{
    public int Id { get; set; }
    public string Hash { get; set; } = default!;
    public BucketItem NumberOfLayers { get; set; } = new BucketItem();
    public List<BucketItem> Layers { get; set; } = new List<BucketItem>();
    public List<BucketItem> Traits { get; set; } = new List<BucketItem>();

    [JsonIgnore]
    public List<BucketItem> GetAll => Layers.Concat(Traits).ToList();
    public OutcomeModel ToModel()
    {
        return new OutcomeModel()
        {
            Id = Id,
            Hash = Hash,
            NumberOfLayers = int.Parse(NumberOfLayers.Name),
            Layers = Layers.Select(l => l.Name).ToList(),
            Traits = Traits.Select(t => t.Name).ToList(),
        };
    }
}

public class OutcomeModel
{
    public int Id { get; set; }
    public string Hash { get; set; } = default!;
    public int NumberOfLayers { get; set; }
    public List<string> Layers { get; set; } = default!;
    public List<string> Traits { get; set; } = default!;
}


