using NFTGenApi.Models;

namespace NFTGenApi.Services.Generator;

public static class Engine
{
    public static List<Buckets> Generate(Properties properties)
    {
        // create matrix
        Matrix matrix = MatrixGenerator.CreateMatrix(properties);

        /*
         * create buckets for each roll type
         * start at how many traits to have.
        */
        List<Buckets> buckets = BucketGenerator.CreateBuckets(properties);

        // testing rolling buckets
        foreach( var bucket in buckets.Where(b => b.Type == CONTAINER_TYPE.TRAIT).ToList())
        {
            while(bucket.BucketList.Count > 0)
            {
                var outcome = bucket.RollBucketList();
            }
        }


        return buckets;
    }
}
