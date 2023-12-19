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



foreach (string line in lines.Skip(ff + 1))
{

    var ls = line[1..(line.Count() -1)].Split(',');

    Dictionary<char, int> values = new();
    foreach (var val in ls)
    {
        var kvp = val.Split('=');
        values.Add(kvp[0][0], int.Parse(kvp[1]));
    }

    parts.Add((values,"in"));
}

for(int i=0; i<parts.Count; i++)
{

    while (parts[i].nextFonk != "R"&& parts[i].nextFonk!="A")
    {
        parts[i] = evaluate(parts[i]);
    }
}

(Dictionary<char, int> vals, string nextFonk)
evaluate((Dictionary<char, int> vals,string nextFonk) part)
{
    (Dictionary<char, int> vals, string nextFonk) ret = (part.vals,"");
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
Console.WriteLine(total);

//px{a<2006:qkq,m>2090:A,rfg}
List<(char c, char cond, int cmp, string next)> parseFonks(string line)
{
    List<(char c, char cond, int cmp, string next)> ret = new();

    line = line[0..(line.Length-1)];
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





/*
 * If a part is sent to another workflow, 
 * it immediately switches to the start of that workflow instead and never returns. 
 * If a part is accepted (sent to A) or rejected (sent to R), the part immediately stops any further
 * 
 * 
 * 
 */

/*
     *  x: Extremely cool looking
        m: Musical (it makes a noise when you hit it)
        a: Aerodynamic
        s: Shiny
 * 
 * */