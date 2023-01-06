using NFTGenApi.Models;

namespace NFTGenApi.Services.Dummy;

public static class GenerateJson
{
    public static Properties GenerateDummyProperties()
    {
        Properties properties = new Properties()
        {
            Seed = Guid.NewGuid(),
            NumberOfItems = 10000,
            LayersProperties = new HashSet<LayersProperty>()
            {
                new LayersProperty()
                {
                    AmountOfLayers = 1,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = true,
                        Value = 100
                    }
                },
                new LayersProperty()
                {
                    AmountOfLayers = 2,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = false,
                        Value = 50
                    }
                },
                new LayersProperty()
                {
                    AmountOfLayers = 3,
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = false,
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
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = false,
                        Value = 25
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Head 1",
                            Constraints = new List<Constraint>() {
                                new Constraint()
                                {
                                    On = TARGET_TYPE.TRAIT,
                                    Name = "Feet 1",
                                    Value = -1
                                }
                            },
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 25
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 2",
                            Constraints = new List<Constraint>() {
                                new Constraint()
                                {
                                    On = TARGET_TYPE.TRAIT,
                                    Name = "Feet 2",
                                    Value = 25
                                }
                            },
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 25
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 25
                            },
                        },
                        new Trait()
                        {
                            Name = "Head 4",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 25
                            },
                        },
                    }
                },
                new Layer()
                {
                    Name = "Body",
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = true,
                        Value = 100
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Body 1",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 24
                            },
                        },
                        new Trait()
                        {
                            Name = "Body 2",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 50
                            },
                        },
                        new Trait()
                        {
                            Name = "Body 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 26
                            },
                        }
                    }
                },
                new Layer()
                {
                    Name = "Feet",
                    Frequency = new Frequency()
                    {
                        Type = FREQ_TYPE.PERCENTAGE,
                        AlwaysOn = false,
                        Value = 33
                    },
                    Traits = new List<Trait>()
                    {
                        new Trait()
                        {
                            Name = "Feet 1",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 12
                            },
                        },
                        new Trait()
                        {
                            Name = "Feet 2",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 28
                            },
                        },
                        new Trait()
                        {
                            Name = "Feet 3",
                            Frequency = new Frequency()
                            {
                                Type = FREQ_TYPE.PERCENTAGE,
                                AlwaysOn = false,
                                Value = 60
                            },
                        }
                    }
                }
            }
        };

        return properties;
    }
}
