using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexMatchVSIsMatch {
	internal static class Program {
		private static readonly Regex TestRegex = new Regex(@"\d", RegexOptions.Compiled | RegexOptions.CultureInvariant); // Testing regex, if string contains at least 1 digit
		static void Main() {
			DoTest(1, 1, true); // Omitting first result because of JIT compilation

			for (int numberPower = 0; numberPower < 5; numberPower++) { // Checking from 1 to 10000 by powers of 10
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
				testStrings.Add(GenerateString(length)); // Filling with random strings
			}

			if (!silent) {
				Console.WriteLine("Measuring...");
			}

			bool firstResult = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start(); // Starting timer for the first test
			foreach (string testString in testStrings) {
				firstResult &= TestRegex.Match(testString).Success;
			}

			stopwatch.Stop(); // Stopping timer for the first test
			long firstElapsedTicks = stopwatch.ElapsedTicks;
			bool secondResult = true;
			stopwatch.Reset();
			stopwatch.Start(); // Starting timer for the second test
			foreach (string testString in testStrings) {
				secondResult &= TestRegex.IsMatch(testString);
			}

			stopwatch.Stop(); // Stopping timer for the second test
			long secondElapsedTicks = stopwatch.ElapsedTicks;
			if (!silent) {
				if (firstResult != secondResult) {
					Console.WriteLine("Stop what"); // That should never happen, but checking anyway
					return;
				}

				Console.WriteLine(firstElapsedTicks < secondElapsedTicks ? $"Match().Success won: {firstElapsedTicks} < {secondElapsedTicks}" : $"IsMatch() won: {firstElapsedTicks} > {secondElapsedTicks}");
			}
		}

		private static readonly Random Random = new Random();
		private static string GenerateString(int length) { // Stole that code from https://stackoverflow.com/a/1344242/9341747
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
		}
	}
}