using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class HighScoreData : ItemData, IHighScoreData, IList<HighScoreData.HighScore>
{
    private readonly List<HighScore> _list;

    public int ScoreType { get; set; }
    public int ClearType { get; set; }

    public int Count => _list.Count;
    public bool IsReadOnly => false;
    public HighScore this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }
    IHighScore IReadOnlyList<IHighScore>.this[int index] => this[index];
    IEnumerator<IHighScore> IEnumerable<IHighScore>.GetEnumerator() => GetEnumerator();

    public HighScoreData()
        : base(ItemDataType.HighScore)
    {
        _list = [];
    }

    public HighScoreData(IHighScoreData data)
        : base(data)
    {
        _list = data.Select(score => new HighScore(score)).ToList();

        ScoreType = data.ScoreType;
        ClearType = data.ClearType;
    }

    protected override void Initialize(in PacketReader p)
    {
        Value = p.Read<string>();
        ScoreType = p.Read<int>();
        ClearType = p.Read<int>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            var highScore = new HighScore
            {
                Value = p.Read<int>()
            };

            int k = p.Read<Length>();
            for (int j = 0; j < k; j++)
            {
                highScore.Names.Add(p.Read<string>());
            }

            Add(highScore);
        }

        // No base call.
    }

    public class HighScore : IHighScore
    {
        public int Value { get; set; }
        public List<string> Names { get; set; } = [];
        IReadOnlyList<string> IHighScore.Names => Names;

        public HighScore() { }
        public HighScore(IHighScore highScore)
        {
            Value = highScore.Value;
            Names = new List<string>(highScore.Names);
        }

        public void Compose(in PacketWriter p)
        {
            p.Write(Value);

            p.Write<Length>(Names?.Count ?? 0);
            if (Names != null)
            {
                foreach (string name in Names)
                    p.Write(name);
            }
        }
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.Write(Value);
        p.Write(ScoreType);
        p.Write(ClearType);

        p.Write<Length>(Count);
        foreach (var highScore in this)
            p.Write(highScore);
    }

    public int IndexOf(HighScore item) => _list.IndexOf(item);
    public void Insert(int index, HighScore item) => _list.Insert(index, item);
    public void RemoveAt(int index) => _list.RemoveAt(index);
    public void Add(HighScore item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public bool Contains(HighScore item) => _list.Contains(item);
    public void CopyTo(HighScore[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public bool Remove(HighScore item) => _list.Remove(item);
    public IEnumerator<HighScore> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
