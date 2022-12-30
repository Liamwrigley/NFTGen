using NFTGenApi.Models;

namespace NFTGenApi.Services;

public static class GenerateJson
{
    public static Properties GenerateDummyProperties()
    {
        Properties properties = new Properties()
        {
            Seed = new Guid().ToString(),
            NumberOfItems = 10000,
            LayersProperties = new HashSet<LayersProperty>()
            {
                new LayersProperty()
                {
                    AmountOfLayers = 1,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.COUNT,
                        Value = 100
                    }
                },
                new LayersProperty()
                {
                    AmountOfLayers = 2,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.COUNT,
                        Value = 200
                    }
                },
                new LayersProperty()
                {
                    AmountOfLayers = 3,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        Value = 50
                    }
                },
            },
            Layers = new List<Layer>() {
                new Layer() 
                {
                    Name = "Head",
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.WEIGHT,
                        Value = 5
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Head 1",
                            Constraints = new List<Constraint>() {
                                new Constraint()
                                {
                                    Type = CONSTRAINT_TYPE.NEVER_WITH,
                                    On = TARGET_TYPE.TRAIT,
                                    Name = "Feet 1"
                                }
                            },
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                Value = 25
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 2",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                Value = 25
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.COUNT,
                                Value = 120
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 4",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 100
                            },
                        },
                    }
                },
                new Layer()
                {
                    Name = "Body",
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.WEIGHT,
                        Value = 5
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Body 1",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        },
                        new Trait()
                        {
                            Name = "Body 2",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        },
                        new Trait()
                        {
                            Name = "Body 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        }
                    }
                },
                new Layer()
                {
                    Name = "Feet",
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.WEIGHT,
                        Value = 5
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Feet 1",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        },
                        new Trait()
                        {
                            Name = "Feet 2",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        },
                        new Trait()
                        {
                            Name = "Feet 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.WEIGHT,
                                Value = 1
                            },
                        }
                    }
                }
            }
        };

        return properties;
    }
}
