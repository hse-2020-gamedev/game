using System;
using System.IO;

public enum PlayerType
{
    Human,
    DummyAI,
    EvilAI
}

[Serializable]
public class GameSettings
{
    public string SceneName;
    public PlayerType[] PlayerTypes;

    public override string ToString()
    {
        using (var stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream);
            Write(writer);
            writer.Flush();

            stream.Position = 0;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd().Replace(Environment.NewLine, ";");
        }
    }

    public static GameSettings Parse(string str)
    {
        using (var stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream);
            writer.Write(str.Replace(";", Environment.NewLine));
            writer.Flush();

            stream.Position = 0;
            var reader = new StreamReader(stream);
            return Read(reader);
        }
    }

    public void Write(StreamWriter writer)
    {
        writer.WriteLine(SceneName);
        writer.WriteLine(PlayerTypes.Length);
        foreach (var type in PlayerTypes)
        {
            writer.WriteLine(type);
        }
    }

    public static GameSettings Read(TextReader reader)
    {
        var result = new GameSettings();
        result.SceneName = reader.ReadLine() ?? throw new NullReferenceException("SceneName is not provided");

        var playerCount = int.Parse(reader.ReadLine()
                                    ?? throw new NullReferenceException("Number of PlayerType's is not provided"));
        result.PlayerTypes = new PlayerType[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            var typeStr = reader.ReadLine()
                          ?? throw new NullReferenceException($"Player type {i} is not provided");
            if (!Enum.TryParse(typeStr, out result.PlayerTypes[i]))
            {
                throw new ArgumentException($"Invalid player type: {typeStr}");
            }
        }

        return result;
    }

    protected bool Equals(GameSettings other)
    {
        return ToString() == other.ToString();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GameSettings) obj);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}
