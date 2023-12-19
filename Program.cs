using System.Diagnostics;

List<string> lines = new();
using (StreamReader reader = new(args[0]))
    while (!reader.EndOfStream)
        lines.Add(reader.ReadLine() ?? "");

Dictionary<string, List<(char c, char cond, int cmp, string next)>> fonks = new();
List<(Dictionary<char, int> vals, string nextFonk)> parts = new();

fonks.Add("A", new() { ('_', '_', 0, "A") });
fonks.Add("R", new() { ('_', '_', 0, "R") });

int ff = 0;
while (lines[ff]!="")
{
    var ls = lines[ff].Split('{');
    fonks.Add(ls[0], parseFonks(ls[1]));
    ff++;
}

part1(ff+1);
part2();


void part2()
{
    ulong ans = 0;

    PriorityQueue<chonk,string> chonks = new();

    chonks.Enqueue(new(new() {
        { 'x',(1,4000)},
        { 'm',(1,4000)},
        { 'a',(1,4000)},
        { 's',(1,4000)},
    })
        ,"in");

    while (chonks.TryDequeue(out chonk restOfIt, out string instruction))
    {
        if (instruction == "A")
        {
            ulong a = 1;
            foreach (var range in restOfIt.v.Values)
            {
                a *= Convert.ToUInt64(range.max - range.min) + 1;
            }
            ans += a;
            continue;
        }
        if (instruction == "R")
            continue;
        foreach (var fonk in fonks[instruction])
        {
            switch (fonk.cond)
            {
                case '_':
                    chonks.Enqueue(restOfIt, fonk.next);
                    break;
                case '<':
                    if (restOfIt.v[fonk.c].min < fonk.cmp)
                    {
                        chonk newChonk = restOfIt.deepCopy();
                        newChonk.setMaxIfLower(fonk.c, fonk.cmp - 1);
                        chonks.Enqueue(newChonk, fonk.next);
                        restOfIt.setMinIfHigher(fonk.c, fonk.cmp);
                        break;
                    }
                    break;
                case '>':
                    if (restOfIt.v[fonk.c].max > fonk.cmp)
                    {
                        chonk newChonk = restOfIt.deepCopy();
                        newChonk.setMinIfHigher(fonk.c, fonk.cmp + 1);
                        chonks.Enqueue(newChonk, fonk.next);
                        restOfIt.setMaxIfLower(fonk.c, fonk.cmp);
                    }
                    break;
            }
            if (fonk.c != '_' && restOfIt.v[fonk.c].min > restOfIt.v[fonk.c].max)
                break;//theres nothing left
        }
    }
    Console.WriteLine($"part2:\t{ans}");
}
void part1(int skipahead)
{

    foreach (string line in lines.Skip(skipahead))
    {

        var ls = line[1..(line.Count() - 1)].Split(',');

        Dictionary<char, int> values = new();
        foreach (var val in ls)
        {
            var kvp = val.Split('=');
            values.Add(kvp[0][0], int.Parse(kvp[1]));
        }

        parts.Add((values, "in"));
    }

    for (int i = 0; i < parts.Count; i++)
    {
        while (parts[i].nextFonk != "R" && parts[i].nextFonk != "A")
        {
            parts[i] = evaluate(parts[i]);
        }
    }

    
    long total = 0;
    foreach (var part in parts)
    {
        if (part.nextFonk == "A")
            foreach (var val in part.vals)
            {
                //Console.WriteLine(val.Value);
                total += val.Value;
            }
        //Console.WriteLine(part);
    }
    Console.WriteLine($"part1:\t{total}");
}

(Dictionary<char, int> vals, string nextFonk)
    evaluate((Dictionary<char, int> vals, string nextFonk) part)
{
    (Dictionary<char, int> vals, string nextFonk) ret = (part.vals, "");
    var fonk = fonks[part.nextFonk];

    foreach (var eval in fonk)
    {
        if (eval.cond == '_')
        {
            ret.nextFonk = eval.next;
            break;
        }
        else if (eval.cond == '<')
        {
            if (part.vals[eval.c] < eval.cmp)
            {
                ret.nextFonk = eval.next;
                break;
            }
            continue;
        }
        else if (eval.cond == '>')
        {
            if (part.vals[eval.c] > eval.cmp)
            {
                ret.nextFonk = eval.next;
                break;
            }
            continue;
        }
        else
        {
            Debug.Assert(false);
        }
    }

    return ret;
}
List<(char c, char cond, int cmp, string next)> parseFonks(string line)
{
    List<(char c, char cond, int cmp, string next)> ret = new();

    line = line[0..(line.Length - 1)];
    var ls = line.Split(",");
    foreach (var condo in ls)
    {
        var cs = condo.Split(':');
        if (cs.Count() == 1)
        {
            ret.Add(('_', '_', 0, cs[0]));
        }
        else if (cs.Count() == 2)
        {
            var csgt = cs[0].Split('>');
            var cslt = cs[0].Split('<');
            if (csgt.Count() == 2)
            {
                ret.Add((csgt[0][0], '>', int.Parse(csgt[1]), cs[1]));
            }
            else if (cslt.Count() == 2)
            {
                ret.Add((cslt[0][0], '<', int.Parse(cslt[1]), cs[1]));
            }
            else
            {
                Debug.Assert(false);
            }
        }
        else
        {
            Debug.Assert(false);
        }
    }

    return ret;
}

record struct chonk
{
    public chonk(Dictionary<char, (int min, int max)> v)
    { this.v = v; }
    public Dictionary<char, (int min, int max)> v;
    public chonk deepCopy()
    {
        return new chonk(v.ToDictionary(x=>x.Key,x=>x.Value));
    }
    public void setMinIfHigher(char c, int min)
    {
        if(min > v[c].min)
            v[c] = (min,v[c].max);
    }
    public void setMaxIfLower(char c, int max)
    {
        if (max < v[c].max)
            v[c] = (v[c].min,max);
    }
}