namespace NFTGenApi.Models;

public enum FREQ_TYPE
{
    WEIGHT = 0,
    COUNT = 1,
    PERCENTAGE = 2
}

//public enum CONSTRAINT_TYPE
//{
//    NEVER_WITH = 0,
//    ALWAYS_WITH = 1,
//}

public enum TARGET_TYPE
{
    LAYER = 0,
    TRAIT = 1,
}

public record Properties
{
    public string Seed { get; init; } = String.Empty;
    public int NumberOfItems { get; init; }
    /// <summary>
    /// Contains the probabilities of a # of traits occuring. e.g. 2 traits: 5%.
    /// </summary>
    public HashSet<LayersProperty> LayersProperties { get; init; } = new HashSet<LayersProperty>();
    public List<Layer> Layers { get; init; } = new List<Layer>();
}
public record LayersProperty
{
    /// <summary>
    /// Number of traits to occur. E.g. 2 traits occuring.
    /// </summary>
    public int AmountOfLayers { get; init; }
    public Frequency Frequency { get; init; } = new Frequency();
}

public interface IPropertyListItem
{
    public string Name { get; init; }
    public Frequency Frequency { get; init; }
    public List<Constraint> Constraints { get; init; }

}

public record Layer : IPropertyListItem
{
    public string Name { get; init; } = String.Empty;
    /// <summary>
    /// The probability of layer to occur
    /// </summary>
    public Frequency Frequency { get; init; } = new Frequency();
    /// <summary>
    /// Any constraints layer has
    /// </summary>
    public List<Constraint> Constraints { get; init; } = new List<Constraint>();
    public List<Trait> Traits { get; init; } = new List<Trait>();

}

public record Trait : IPropertyListItem
{
    public string Name { get; init; } = String.Empty;
    /// <summary>
    /// The probability of layer to occur
    /// </summary>
    public Frequency Frequency { get; init; } = new Frequency();
    /// <summary>
    /// Any constraints layer has
    /// </summary>
    public List<Constraint> Constraints { get; init; } = new List<Constraint>();
}


public record Frequency
{
    public FREQ_TYPE Type { get; init; }
    public decimal Value { get; init; }
}

public record Constraint
{
    //public CONSTRAINT_TYPE Type { get; init; }
    public TARGET_TYPE On { get; init; }
    public string Name { get; init; } = String.Empty;
    /// <summary>
    /// -1 = never with
    /// 0 = default
    /// 0> - <1 = percentage alterations
    /// 1 = always with
    /// </summary>
    public decimal Value { get; init; } = 0M;

}
