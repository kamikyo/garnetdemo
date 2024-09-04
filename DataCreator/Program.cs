using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO.Pipelines;

namespace DataCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var redis = ConnectionMultiplexer.Connect("127.0.0.1:7010");
            var db = redis.GetDatabase(0);
            
            int length = 5000;
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                List<Task> pipe = new List<Task>();
                for (int i = 0; i < length; i++) 
                {
                    MyData data = new MyData
                    { 
                        TimeStamp = DateTime.Now,
                        C1 = i % 2 == 0 ? true : false,
                        C2 = i,
                        C3 = i,
                        C4 = DateTime.Now,
                        C5 = i % 2 == 0 ? DateTime.Now : null,
                        C6 = i,
                        C7 = i,
                        C8 = i,
                        C9 = i % 2 == 0 ? i : null,
                        C10 = i.ToString(),
                        C11 = (i % 3) switch { 0 => DbOperation.Insert, 1 => DbOperation.Update, 2 => DbOperation.Delete, _ => DbOperation.Insert }
                    };

                    pipe.Add(db.SortedSetAddAsync(data.ID,
                            data.ID, data.TimeStamp.Ticks, CommandFlags.None));
                    pipe.Add(db.StringSetAsync(data.ID, System.Text.Json.JsonSerializer.Serialize(data)));
                    pipe.Add(db.SortedSetAddAsync("allcards", data.C2, 0, CommandFlags.None));
                }
                Task.WaitAll(pipe.ToArray());
                Console.WriteLine("Time taken:{0}", sw.ElapsedMilliseconds);
                Thread.Sleep(3000);
            }
        }
    }

    public class MyData
    {
        internal string ID { get; set; } = Guid.NewGuid().ToString();

        internal DateTime TimeStamp { get; set; } = DateTime.Now;

        internal bool C1 { get; set; } = false;
        internal int C2 { get; set; }
        internal int C3 { get; set; }
        internal DateTime C4 { get; set; }
        internal DateTime? C5 { get; set; }
        internal int C6 { get; set; }
        internal int C7 { get; set; }
        internal double C8 { get; set; }
        internal int? C9 { get; set; }
        internal string C10 { get; set; }
        internal DbOperation C11 { get; set; }
    }

    internal enum DbOperation
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
    }
}
