namespace ci_common_qa_automation.Utilities
{
    public class RandomAnswerGenerator
    {
        Random r;
        public RandomAnswerGenerator()
        {
            r = new Random();
        }
        public RandomAnswerGenerator(Random random)
        {
            r = random;
        }
        public string GenerateRandomWords(string mask, int numberOfWords)
        {
            string result;
            char[] availableLetters = "ABCDEFGHIJKLMNOPQRSTUVWXWZabcdefghijklmnopqrstuvwxyz.".ToCharArray();
            char[] availableNumbers = "0123456789".ToCharArray();
            switch (mask)
            {
                case "99999"://Zipcode
                    result = GetRandomnessSpecificLength(availableNumbers, 1, 5);
                    break;
                case "(999)999-9999"://Phone
                    result = "(" + GetRandomnessSpecificLength(availableNumbers, 1, 3) + ")" + GetRandomnessSpecificLength(availableNumbers, 1, 3) + "-" + GetRandomnessSpecificLength(availableNumbers, 1, 4);
                    break;
                //case "*{1,64}[.*{1,64}][.*{1,64}][.*{1,64}]@*{1,64}[.*{2,64}][.*{2,6}][.*{1,2}]"://Email
                case "@":
                    int numberCharacters = r.Next(3, 12);
                    result = GetRandomness(availableLetters, 1, numberCharacters);
                    numberCharacters = r.Next(3, 12);
                    result = result + "@" + GetRandomness(availableLetters, 1, numberCharacters);
                    numberCharacters = r.Next(2, 3);
                    result = result + "." + GetRandomness(availableLetters, 1, numberCharacters);
                    break;
                default:
                    numberCharacters = r.Next(5, 20);
                    result = GetRandomness(availableLetters, numberOfWords, numberCharacters);
                    break;
            }
            result.TrimEnd();
            return result;
        }
        public string GenerateRandomNumber(int numberOfCharacters)
        {
            char[] availableNumbersNoZero = "123456789".ToCharArray();
            char[] availableNumbers = "0123456789".ToCharArray();
            string number = GetRandomness(availableNumbersNoZero, 1, 1);
            number = number + GetRandomness(availableNumbers, 1, numberOfCharacters - 1);
            return number;
        }
        public string GenerateRandomNumber(int numDigits, bool specificLength)
        {
            char[] allNumbers = "0123456789".ToCharArray();
            char[] noZero = "123456789".ToCharArray();
            string results = GetRandomnessSpecificLength(noZero, 1, 1);
            if (specificLength) { results = results + GetRandomnessSpecificLength(allNumbers, 1, numDigits - 1); }
            else { results = results + GetRandomness(allNumbers, 1, numDigits - 1); }
            return results;
        }
        public string GenerateRandomRealNumber(int numberOfCharacters)
        {
            char[] availableNumbersNoZero = "123456789".ToCharArray();
            char[] availableNumbers = "0123456789".ToCharArray();
            string number = GetRandomness(availableNumbersNoZero, 1, 1);
            number = number + GetRandomness(availableNumbers, 1, 3) + ".00";
            return number;
        }
        public string GenerateRandomDecimal(int numDigits, int numDecimals, bool specificLength)
        {
            char[] allNumbers = "0123456789".ToCharArray();
            char[] noZero = "123456789".ToCharArray();
            string results = GetRandomnessSpecificLength(noZero, 1, 1);
            if (specificLength) { results = results + GetRandomnessSpecificLength(allNumbers, 1, numDigits - 1); }
            else { results = results + GetRandomness(allNumbers, 1, numDigits - 1); }
            if (specificLength) { results = numDecimals != 0 ? results + "." + GetRandomnessSpecificLength(allNumbers, 1, numDecimals) : results; }
            else { results = numDecimals != 0 ? results + "." + GetRandomness(allNumbers, 1, numDecimals) : results; }

            return results;
        }
        public string GenerateRandomDecimalAllDecimals(int numDigits, int numDecimals, bool specificLength)
        {
            char[] allNumbers = "0123456789".ToCharArray();
            char[] noZero = "123456789".ToCharArray();
            string results = GetRandomnessSpecificLength(noZero, 1, 1);
            if (specificLength) { results = results + GetRandomnessSpecificLength(allNumbers, 1, numDigits - 1); }
            else { results = results + GetRandomness(allNumbers, 1, numDigits - 1); }
            results = numDecimals != 0 ? results + "." + GetRandomnessSpecificLength(allNumbers, 1, numDecimals) : results;

            return results;
        }
        public string GenerateRandomDecimalPercentage(int numDecimals, bool specificLength)
        {
            char[] allNumbers = "0123456789".ToCharArray();
            string results = "0";
            if (specificLength) { results = numDecimals != 0 ? results + "." + GetRandomnessSpecificLength(allNumbers, 1, numDecimals) : results; }
            else { results = numDecimals != 0 ? results + "." + GetRandomness(allNumbers, 1, numDecimals) : results; }

            return results;
        }
        public string GenerateRandomDate()
        {
            DateTime from = new(1990, 01, 01);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            return newDate.Date.ToString("yyyy-MM-dd");
        }

        public string GenerateRandomTime()
        {
            DateTime from = new(1990, 01, 01);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            return newDate.ToString("HH:mm:ss");
        }
        public string GenerateRandomDateGreaterThan(string startDate)
        {
            DateTime from = DateTime.Parse(startDate);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            return newDate.Date.ToString("yyyy/MM/dd");
            //return newDate.Date.ToString("MM/dd/yyyy");
        }
        public string GenerateRandomDateGreaterThan(string startDate, string format)
        {
            int randomDays = r.Next(5, 20);
            DateTime newDate = DateTime.Parse(startDate).AddDays(randomDays);
            return newDate.Date.ToString(format);
        }
        public string GenerateRandomDateLessThan(string endDate, string format)
        {
            int randomDays = r.Next(5, 20);
            DateTime newDate = DateTime.Parse(endDate).AddDays(-randomDays);
            return newDate.Date.ToString(format);
        }
        public string GenerateRandomDateTime()
        {
            DateTime from = new(1980, 01, 01);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            return newDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string GenerateRandomSmallDateTime()
        {
            DateTime from = new(1980, 01, 01);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            return newDate.ToString("yyyy-MM-dd HH:mm:") + "00";
        }
        public string GetRandomDateTimeWithTz()
        {
            DateTime from = new(1980, 01, 01);
            DateTime to = DateTime.Now;
            TimeSpan range = to - from;
            TimeSpan randTimePan = new((long)(r.NextDouble() * range.Ticks));
            DateTime newDate = from + randTimePan;
            DateTimeOffset tz = newDate;
            int randomTz = r.Next(-12, 0);
            tz = tz.ToOffset(new TimeSpan(randomTz, 0, 0));
            return tz.ToString("yyyy-MM-dd HH:mm:ss zzz");
        }
        public string GenerateRandomTrueFalse()
        {
            int random = r.Next(0, 2);
            string result = random == 0 ? "True" : "False";
            return result;
        }
        public string GenerateRandomBit()
        {
            int random = r.Next(0, 2);
            return random == 1 ? "1" : "0";
        }
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public string GenerateCharNumberString(int numChars)
        {
            char[] availableLetters = "ABCDEFGHIJKLMNOPQRSTUVWXWZabcdefghijklmnopqrstuvwxyz.".ToCharArray();
            char[] availableNumbers = "0123456789".ToCharArray();
            int half1 = numChars / 2;
            int half2 = numChars - half1;
            string result = GetRandomness(availableNumbers, 1, half1) + GetRandomness(availableLetters, 1, half2);
            return result;
        }
        public T GetRandomItemFromList<T>(List<T> list)
        {
            int randomIndex = r.Next(0, list.Count());
            return list[randomIndex];
        }

        public T GetRandomItemFromList<T>(ref List<T> list, T item)
        {
            List<T> unquieList = list.Distinct().ToList();
            int index = unquieList.IndexOf(item);
            if (unquieList.Count - 1 != index) return unquieList[index + 1];
            ShuffleList(ref list);
            return list[0];
        }

        public T GetNextItemFromList<T>(List<T> list, T item)
        {
            List<T> unquieList = list.Distinct().ToList();
            int index = unquieList.IndexOf(item);
            if (unquieList.Count - 1 != index) return unquieList[index + 1];
            return list[0];
        }
        public List<int> GetRandomNumbers(int start, int end, int numberToReturn)
        {
            List<int> result = new();
            if ((end - start) < numberToReturn)
            {
                for (int i = start; i <= end; i++) { result.Add(i); }
            }
            else
            {
                bool run = true;
                while (run)
                {
                    int value = r.Next(start, end);
                    if (result.Contains(value)) continue;
                    result.Add(value);
                    if (result.Count == numberToReturn) run = false;
                }
            }

            return result;
        }
        public void ShuffleList<TE>(ref List<TE> inputList)
        {
            List<TE> randomList = new();
            while (inputList.Count > 0)
            {
                int randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            inputList = randomList;
        }

        public void AdvaceListIndex<TE>(ref List<TE> inputList)
        {
            if (inputList.Count == 1) return;
            TE firstValue = inputList.First();
            inputList.RemoveAt(0);
            inputList.Add(firstValue);
        }
        public string GetRandomnessSpecificLength(char[] availableCharacters, int numberOfWords, int numberCharacters)
        {
            string result = "";
            for (int i = 1; i <= numberOfWords; i++)
            {
                string word = "";
                for (int j = 1; j <= numberCharacters; j++)
                {
                    int letter = r.Next(0, availableCharacters.Length - 1);
                    result += availableCharacters[letter];
                }
                result = result + word + " ";
            }
            return result.TrimEnd();
        }
        public string GetRandomness(char[] availableCharacters, int numberOfWords, int numberCharacters)
        {
            string result = "";

            for (int i = 1; i <= numberOfWords; i++)
            {
                const string word = "";
                int randomNumberCharacters = numberCharacters > 0 ? numberCharacters < 11 ? r.Next(1, numberCharacters) : r.Next(10, numberCharacters) : 0;
                for (int j = 1; j <= randomNumberCharacters; j++)
                {
                    int letter = r.Next(0, availableCharacters.Length - 1);
                    result += availableCharacters[letter];
                }
                result = result + word + " ";
            }
            return result.TrimEnd();
        }
    }
}
