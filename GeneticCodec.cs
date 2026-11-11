using System.Text;

namespace lab1;

public struct GeneticData
{
    public string ProteinName;
    public string OrganismName;
    public string AminoAcids;

    public static GeneticData FromString(string line)
    {
        string[] fields = line.Split('\t', 3);
        if (fields.Length != 3 || fields.Any(string.IsNullOrWhiteSpace))
            throw new FormatException("A sequence record must contain protein, organism, and amino-acid data separated by tabs.");

        return new GeneticData
        {
            ProteinName = fields[0],
            OrganismName = fields[1],
            AminoAcids = GeneticCodec.Decode(fields[2])
        };
    }
}

public static class GeneticCodec
{
    public static List<GeneticData> ReadFile(string path)
    {
        List<GeneticData> data = [];
        foreach (string line in File.ReadLines(path))
            if (!string.IsNullOrWhiteSpace(line))
                data.Add(GeneticData.FromString(line));
        return data;
    }

    public static string Decode(string value)
    {
        StringBuilder result = new(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] is < '0' or > '9')
            {
                result.Append(value[i]);
                continue;
            }

            int count = value[i] - '0';
            if (count == 0 || i + 1 == value.Length || char.IsAsciiDigit(value[i + 1]))
                throw new FormatException($"Invalid RLE sequence near position {i}.");

            result.Append(value[++i], count);
        }
        return result.ToString();
    }
}
