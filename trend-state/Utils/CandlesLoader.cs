using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace trend_state.Controllers
{
    public class CandlesLoader
    {
        private string _fileName;

        public CandlesLoader(string fileName)
        {
            this._fileName = fileName;
        }

        public IEnumerable<Candle> LoadAll()
        {
            var candles = new List<Candle>();
            using (StreamReader sr = new StreamReader(_fileName))
            {
                var lines = ReadLines(sr).ToList();
                foreach (var line in lines)
                {
                    var arr = line.Split(';');
                    candles.Add(new Candle()
                    {
                        Date = DateTime.ParseExact(arr[0], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture),
                        Open = float.Parse(arr[1], CultureInfo.InvariantCulture),
                        High = float.Parse(arr[2], CultureInfo.InvariantCulture),
                        Low = float.Parse(arr[3], CultureInfo.InvariantCulture),
                        Close = float.Parse(arr[4], CultureInfo.InvariantCulture)
                    });
                }
            }

            return candles;
        }

        private IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}