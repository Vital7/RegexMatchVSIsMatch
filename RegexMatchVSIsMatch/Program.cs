using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexMatchVSIsMatch {
	internal static class Program {
		private static readonly Regex TestRegex = new Regex(@"\d", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		static void Main() {
			DoTest(1, 1, true); // Omitting first result because of JIT compilation
			for (int numberPower = 0; numberPower < 5; numberPower++) { // Checking from 1 
				int number = (int) Math.Pow(10, numberPower);
				for (int lengthPower = 0; lengthPower < 5; lengthPower++) {
					int length = (int) Math.Pow(10, lengthPower);
					DoTest(number, length);
				}
			}
		}

		static void DoTest(int number, int length, bool silent = false) {
			if (!silent) {
				Console.WriteLine("-------------------------------------------");
				Console.WriteLine($"Generating {number} of {length}-symbol strings");
			}

			HashSet<string> testStrings = new HashSet<string>(number);
			for (int i = 0; i < number; i++) {
				testStrings.Add(GenerateString(length));
			}

			if (!silent) {
				Console.WriteLine("Measuring...");
			}

			bool firstResult = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (string testString in testStrings) {
				firstResult &= TestRegex.Match(testString).Success;
			}

			stopwatch.Stop();
			long firstElapsedTicks = stopwatch.ElapsedTicks;
			bool secondResult = true;
			stopwatch.Reset();
			stopwatch.Start();
			foreach (string testString in testStrings) {
				secondResult &= TestRegex.IsMatch(testString);
            }

			stopwatch.Stop();
			long secondElapsedTicks = stopwatch.ElapsedTicks;
			if (!silent) {
				if (firstResult != secondResult) {
					Console.WriteLine("Stop what");
					return;
				}

				Console.WriteLine(firstElapsedTicks < secondElapsedTicks ? $"Match().Success won: {firstElapsedTicks} < {secondElapsedTicks}" : $"IsMatch() won: {firstElapsedTicks} > {secondElapsedTicks}");
			}
		}

		private static readonly Random Random = new Random();
		private static string GenerateString(int length) {
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
		}
	}
}