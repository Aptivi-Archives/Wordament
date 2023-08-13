/*
 * MIT License
 * 
 * Copyright (c) 2023 Aptivi
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wordament
{
    /// <summary>
    /// The word management class
    /// </summary>
    public static class WordManager
    {
        private static readonly List<string> Words = new();
        private static readonly HttpClient httpClient = new();
        private static readonly Random rng = new();

        /// <summary>
        /// Initializes the words. Does nothing if already downloaded.
        /// </summary>
        public static void InitializeWords()
        {
            // Download the words
            if (Words.Count == 0)
            {
                var response = httpClient.GetAsync("https://cdn.jsdelivr.net/gh/dwyl/english-words/words_alpha.txt").Result;
                var contentStream = new MemoryStream();
                response.Content.ReadAsStreamAsync().Result.CopyTo(contentStream);
                contentStream.Seek(0L, SeekOrigin.Begin);
                Words.AddRange(new StreamReader(contentStream).ReadToEnd().SplitNewLines().ToList());
            }
        }

        /// <summary>
        /// Initializes the words. Does nothing if already downloaded.
        /// </summary>
        public static async Task InitializeWordsAsync()
        {
            // Download the words
            if (Words.Count == 0)
            {
                var response = await httpClient.GetAsync("https://cdn.jsdelivr.net/gh/dwyl/english-words/words_alpha.txt");
                var contentStream = new MemoryStream();
                var stream = await response.Content.ReadAsStreamAsync();
                stream.CopyTo(contentStream);
                contentStream.Seek(0L, SeekOrigin.Begin);
                Words.AddRange(new StreamReader(contentStream).ReadToEnd().SplitNewLines().ToList());
            }
        }

        /// <summary>
        /// Gets all words
        /// </summary>
        public static string[] GetWords()
        {
            InitializeWords();
            return Words.ToArray();
        }

        /// <summary>
        /// Gets a random word
        /// </summary>
        /// <returns>A random word</returns>
        public static string GetRandomWord()
        {
            InitializeWords();
            return Words[rng.Next(Words.Count)];
        }

        /// <summary>
        /// Gets a random word conditionally
        /// </summary>
        /// <param name="wordMaxLength">The maximum length of the word</param>
        /// <param name="wordStartsWith">The word starts with...</param>
        /// <param name="wordEndsWith">The word ends with...</param>
        /// <param name="wordExactLength">The exact length of the word</param>
        /// <returns>A random word</returns>
        public static string GetRandomWordConditional(int wordMaxLength, string wordStartsWith, string wordEndsWith, int wordExactLength)
        {
            // Get an initial word
            string word = GetRandomWord();
            bool lengthCheck = wordMaxLength > 0;
            bool startsCheck = !string.IsNullOrWhiteSpace(wordStartsWith);
            bool endsCheck = !string.IsNullOrWhiteSpace(wordEndsWith);
            bool exactLengthCheck = wordExactLength > 0;

            // Loop until all the conditions that need to be checked are satisfied
            while (!(((lengthCheck && word.Length <= wordMaxLength) || !lengthCheck) &&
                     ((startsCheck && word.StartsWith(wordStartsWith)) || !startsCheck) &&
                     ((endsCheck && word.EndsWith(wordEndsWith)) || !endsCheck) &&
                     ((exactLengthCheck && word.Length == wordExactLength) || !exactLengthCheck)))
                word = GetRandomWord();

            // Get a word that satisfies all the conditions
            return word;
        }

        /// <summary>
        /// Gets all words
        /// </summary>
        public static async Task<string[]> GetWordsAsync()
        {
            await InitializeWordsAsync();
            return Words.ToArray();
        }

        /// <summary>
        /// Gets a random word
        /// </summary>
        /// <returns>A random word</returns>
        public static async Task<string> GetRandomWordAsync()
        {
            await InitializeWordsAsync();
            return Words[rng.Next(Words.Count)];
        }

        /// <summary>
        /// Gets a random word conditionally
        /// </summary>
        /// <param name="wordMaxLength">The maximum length of the word</param>
        /// <param name="wordStartsWith">The word starts with...</param>
        /// <param name="wordEndsWith">The word ends with...</param>
        /// <param name="wordExactLength">The exact length of the word</param>
        /// <returns>A random word</returns>
        public static async Task<string> GetRandomWordConditionalAsync(int wordMaxLength, string wordStartsWith, string wordEndsWith, int wordExactLength)
        {
            // Get an initial word
            string word = await GetRandomWordAsync();
            bool lengthCheck = wordMaxLength > 0;
            bool startsCheck = !string.IsNullOrWhiteSpace(wordStartsWith);
            bool endsCheck = !string.IsNullOrWhiteSpace(wordEndsWith);
            bool exactLengthCheck = wordExactLength > 0;

            // Loop until all the conditions that need to be checked are satisfied
            while (!(((lengthCheck && word.Length <= wordMaxLength) || !lengthCheck) &&
                     ((startsCheck && word.StartsWith(wordStartsWith)) || !startsCheck) &&
                     ((endsCheck && word.EndsWith(wordEndsWith)) || !endsCheck) &&
                     ((exactLengthCheck && word.Length == wordExactLength) || !exactLengthCheck)))
                word = GetRandomWord();

            // Get a word that satisfies all the conditions
            return word;
        }

        private static string[] SplitNewLines(this string Str) =>
            Str.Replace(Convert.ToChar(13).ToString(), "")
               .Split(Convert.ToChar(10));
    }
}
