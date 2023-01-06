using Newtonsoft.Json;
using NFTGenApi.Models;
using static NFTGenApi.Services.Helpers.Helpers;

namespace NFTGenApi.Services.Generator;

public static class MatrixGenerator
{
    public static Matrix CreateMatrix(Properties properties)
    {
        // generate property lists
        List<(IPropertyListItem, TARGET_TYPE)> layers = GetPropertyList(properties, TARGET_TYPE.LAYER);
        List<(IPropertyListItem, TARGET_TYPE)> traits = GetPropertyList(properties, TARGET_TYPE.TRAIT);
        List<(IPropertyListItem, TARGET_TYPE)> combined = layers.Concat(traits).ToList();

        // generate key mappings for each type
        Dictionary<(string, TARGET_TYPE), int> mappings = GenerateKeysMapping(combined);

        // generate adjacency matrix
        Matrix matrix = GenerateMatrix(combined, mappings);

        return matrix;
    }


    public static Matrix GenerateMatrix(List<(IPropertyListItem, TARGET_TYPE)> propertyList, Dictionary<(string, TARGET_TYPE), int> mapping)
    {
        var c = propertyList.Count;
        decimal[,] matrix = new decimal[c, c];

        foreach (var item in propertyList)
        {
            foreach (var constraint in item.Item1.Constraints)
            {
                TARGET_TYPE callingType = item.Item2;
                TARGET_TYPE targetType = constraint.On;
                string callingName = item.Item1.Name;
                string targetName = constraint.Name;

                var callingIndex = mapping[(callingName, callingType)];
                var targetIndex = mapping[(targetName, targetType)];
                matrix[callingIndex, targetIndex] = IntToDecimal(constraint.Value);
            }
        }
        return new Matrix(matrix, mapping);
    }

    private static List<(IPropertyListItem, TARGET_TYPE)> GetPropertyList(Properties properties, TARGET_TYPE type) => type switch
    {
        TARGET_TYPE.TRAIT => GetTraits(properties, TARGET_TYPE.TRAIT),
        TARGET_TYPE.LAYER => GetLayers(properties, TARGET_TYPE.LAYER),
        _ => throw new NotSupportedException()
    };

    private static List<(IPropertyListItem, TARGET_TYPE)> GetTraits(Properties properties, TARGET_TYPE type)
    {
        var list = new List<(IPropertyListItem, TARGET_TYPE)>();
        foreach (var layer in properties.Layers)
        {
            foreach (var trait in layer.Traits)
            {

                list.Add((trait, type));
            }
        }
        return list;
    }
    private static List<(IPropertyListItem, TARGET_TYPE)> GetLayers(Properties properties, TARGET_TYPE type)
    {
        var list = new List<(IPropertyListItem, TARGET_TYPE)>();
        foreach (var layer in properties.Layers)
        {
            list.Add((layer, type));
        }
        return list;
    }

    private static Dictionary<(string, TARGET_TYPE), int> GenerateKeysMapping(List<(IPropertyListItem, TARGET_TYPE)> elements)
    {
        Dictionary<(string, TARGET_TYPE), int> mapping = new Dictionary<(string, TARGET_TYPE), int>();
        int i = 0;
        foreach ((IPropertyListItem, TARGET_TYPE) element in elements)
        {
            mapping[(element.Item1.Name, element.Item2)] = i;
            ++i;
        }

        return mapping;
    }
};

public class Matrix
{
    public decimal[,] _matrix;
    public Dictionary<(string, TARGET_TYPE), int> _keyToIndex;

    public Matrix(decimal[,] matrix, Dictionary<(string, TARGET_TYPE), int> keyToIndex)
    {
        this._matrix = matrix;
        this._keyToIndex = keyToIndex;
    }

    public int GetIdFromKey(string key, TARGET_TYPE type)
    {
        return _keyToIndex[(key, type)];
    }

    public (string, TARGET_TYPE) GetKeyFromId(int id)
    {
        return _keyToIndex.FirstOrDefault(x => x.Value == id).Key;
    }

    public bool HasEdge(string key1, TARGET_TYPE type1, string key2, TARGET_TYPE type2)
    {
        int index1 = _keyToIndex[(key1, type1)];
        int index2 = _keyToIndex[(key2, type2)];
        return _matrix[index1, index2] == 1;
    }

    public MatrixModel ToModel()
    {
        return new MatrixModel()
        {
            matrix = JsonConvert.SerializeObject(this._matrix),
            keyToIndex = JsonConvert.SerializeObject(_keyToIndex)
        };
    }
}

public class MatrixModel
{
    public string matrix { get; set; } = string.Empty;
    public string keyToIndex { get; init; } = string.Empty;
}
