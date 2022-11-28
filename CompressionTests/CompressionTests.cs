using CompressionLibrary;
using System.Diagnostics;
using System.Globalization;

namespace CompressionTests
{
    public class Tests
    {
        Compression compression;
        DateTime start, end;
        [SetUp] public void SetUp()
        {
            RepeatItem.Delimiter = "";
            compression = new Compression();
        }
        private void Permutate_Validate_Compression(string input, string expected)
        {
            lock (this)
            {
                bool Test = false, Pattern = false;
                RunCombination(Test, Pattern, input, expected);
                Test = true;
                RunCombination(Test, Pattern, input, expected);
                Test = false; Pattern = true;
                RunCombination(Test, Pattern, input, expected);
                Test = true;
                RunCombination(Test, Pattern, input, expected);
            }
        }
        private void RunCombination(bool Test, bool Pattern, string input, string expected)
        {
            Console.WriteLine($"Dictionary Reference: {Test} | Dynamic Programming Struct: {Pattern}");
            Validate_Compression(input, expected, Test, Pattern);
            Console.WriteLine();

        }
        private void Validate_Compression(string input, string expected, bool Test, bool Pattern)
        {
            Compression.baseCaseCount = 0; Compression.dictionaryHitCount = 0; Compression.recursiveHitCount = 0; Compression.solutionHitCount = 0;
            Compression.Test = Test;
            Compression.Pattern = Pattern;
            Compression.PatternDictionary = new RepeatPatternDictionary();
            Compression.PreviousComputations = new Dictionary<string, List<object>>();
            start = DateTime.Now;
            string evaluation = compression.Compress(input);
            end = DateTime.Now;
            TimeSpan ts = end - start;

            Assert.AreEqual(expected, evaluation);

            var prior = input.Length;
            var current = evaluation.Length;
            var compressionPercent = ((double)current / prior);
            Console.WriteLine($"Compressed {input} to {evaluation}, resulting in {(compressionPercent * 100.0):n2}% compressed size compared to original");
            Console.WriteLine($"Base Case count: {Compression.hitCount} | Normal Dictionary Reference count: {Compression.altHitCount} | Recursive count: {Compression.recursiveHitCount} | DP Solution count: {Compression.solutionHitCount}");
            Console.WriteLine($"Total hit count: {Compression.hitCount + Compression.altHitCount + Compression.recursiveHitCount}");
            Console.WriteLine($"Elapsed Time is {ts.TotalMilliseconds}ms");
        }

        [TestCase("aaaaaaaa", "8[a]")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "26[a]")]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "13[a]13[b]")]
        public void Compress_SimpleCompress_Test(string uncompress, string compress)
        {
            Permutate_Validate_Compression(uncompress, compress);
        }
        
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "~26[a]")]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "~13[a]~13[b]")]
        public void Compress_Delimiter_Test(string uncompress, string compress)
        {
            RepeatItem.Delimiter = "~";
            Permutate_Validate_Compression(uncompress, compress);
        }
        
        [TestCase("abababababababababababababababababababababababababab", "~26[ab]")]
        [TestCase("abcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabc", "~26[abc]")]
        public void Compress_IncreasingSize_Test(string uncompress, string compress)
        {
            RepeatItem.Delimiter = "~";
            Permutate_Validate_Compression(uncompress, compress);
        }

        [TestCase("aabcabcaabcabcaabcabcaabcabc", "4[a2[abc]]")]
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "8[a2[abc]]")]
        //[TestCase("aabcabcaabcabcaabcabcaabcabc", "aa2[2[bca]a]2[bca]2[abc]")] //alt compression
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "2[aa2[2[bca]a]2[bca]2[abc]]")] //alt compression
        //[TestCase("abcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbb", "3[2[abc]13[b]]")]
        public void Compress_AdvancedCompress_Test(string uncompress, string compress)
        {
            Permutate_Validate_Compression(uncompress, compress);
        }

        [TestCase("dabababcdcdcd", "d3[ab]3[cd]")]
        [TestCase("dabababcdcdcddabababcdcdcd", "2[d3[ab]3[cd]]")]
        public void Compress_Multiple_Test(string uncompress, string compress)
        {
            Permutate_Validate_Compression(uncompress, compress);
        }

        [TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d3[ab]5[cb]c]")]
        //[TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d2[ab]a6[bc]]")] //alternative which is supposed to fail
        public void Compress_EquivalentExpression_Test(string uncompress, string expected)
        {
            Permutate_Validate_Compression(uncompress, expected);
        }
    }
}