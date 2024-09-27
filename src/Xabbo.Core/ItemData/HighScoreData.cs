using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IHighScoreData"/>
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
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        Value = p.ReadString();
        ScoreType = p.ReadInt();
        ClearType = p.ReadInt();
        _list.AddRange(p.ParseArray<HighScore>());

        // No base call.
    }

    public class HighScore : IHighScore, IParserComposer<HighScore>
    {
        public int Score { get; set; }
        public List<string> Names { get; set; }
        IReadOnlyList<string> IHighScore.Users => Names;

        public HighScore()
        {
            Names = [];
        }

        public HighScore(IHighScore highScore)
        {
            Score = highScore.Score;
            Names = [.. highScore.Users];
        }

        private HighScore(in PacketReader p)
        {
            Score = p.ReadInt();
            Names = [.. p.ReadStringArray()];
        }

        void IComposer.Compose(in PacketWriter p)
        {
            p.WriteInt(Score);
            p.WriteStringArray(Names);
        }

        static HighScore IParser<HighScore>.Parse(in PacketReader p) => new(in p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteString(Value);
        p.WriteInt(ScoreType);
        p.WriteInt(ClearType);
        p.ComposeArray<HighScore>(this);
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
