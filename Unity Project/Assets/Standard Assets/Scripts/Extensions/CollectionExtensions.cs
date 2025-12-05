using System;
using Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Extensions
{
	public static class CollectionExtensions 
	{
		public static Vector2[] ToVec2s (this _Vector2[] arr)
		{
			Vector2[] output = new Vector2[arr.Length];
			for (int i = 0; i < arr.Length; i ++)
				output[i] = arr[i].ToVec2();
			return output;
		}

		public static _Vector2[] FromVec2s (this Vector2[] arr)
		{
			_Vector2[] output = new _Vector2[arr.Length];
			for (int i = 0; i < arr.Length; i ++)
				output[i] = _Vector2.FromVec2(arr[i]);
			return output;
		}

		public static List<T> Randomize<T> (this T[] arr)
		{
			List<T> output = new List<T>();
			List<T> eltsLeft = new List<T>(arr);
			while (eltsLeft.Count > 0)
			{
				int randIdx = Random.Range(0, eltsLeft.Count);
				output.Add(eltsLeft[randIdx]);
				eltsLeft.RemoveAt(randIdx);
			}
			return output;
		}

		public static T[] Add<T> (this T[] arr, T element)
		{
			List<T> output = new List<T>(arr);
			output.Add(element);
			return output.ToArray();
		}

		public static T[] Remove<T> (this T[] arr, T element)
		{
			List<T> output = new List<T>(arr);
			output.Remove(element);
			return output.ToArray();
		}

		public static T[] RemoveAt<T> (this T[] arr, int index)
		{
			List<T> output = new List<T>(arr);
			output.RemoveAt(index);
			return output.ToArray();
		}

		public static T[] AddRange<T> (this T[] arr, IEnumerable<T> arr2)
		{
			List<T> output = new List<T>(arr);
			output.AddRange(arr2);
			return output.ToArray();
		}

		public static bool Contains<T> (this T[] arr, T element)
		{
			for (int i = 0; i < arr.Length; i ++)
			{
				T ob = arr[i];
				if (ob == null)
				{
					if (element == null)
						return true;
				}
				else if (ob.Equals(element))
					return true;
			}
			return false;
		}

		public static int IndexOf<T> (this T[] arr, T element)
		{
			for (int i = 0; i < arr.Length; i ++)
			{
				T ob = arr[i];
				if (ob == null)
				{
					if (element == null)
						return i;
				}
				if (ob.Equals(element))
					return i;
			}
			return -1;
		}
		
		public static T[] Reverse<T> (this T[] arr)
		{
			List<T> output = new List<T>(arr);
			output.Reverse();
			return output.ToArray();
		}

		public static T[] AddArray<T> (this T[] arr, Array arr2)
		{
			List<T> output = new List<T>(arr);
			for (int i = 0; i < arr2.Length; i ++)
				output.Add((T) arr2.GetValue(i));
			return output.ToArray();
		}

		public static string ToString<T> (this T[] arr, string elementSeperator = ", ")
		{
            string output = "";
			for (int i = 0; i < arr.Length; i ++)
			{
				T element = arr[i];
                output += element.ToString() + elementSeperator;
			}
			return output;
		}

		public static List<T> RemoveEach<T> (this List<T> list, IEnumerable<T> arr)
		{
			List<T> output = new List<T>(list);
			foreach (T element in arr)
				output.Remove(element);
			return output;
		}

		public static T[] RemoveEach<T> (this T[] arr, IEnumerable<T> arr2)
		{
			return new List<T>(arr).RemoveEach(arr2).ToArray();
		}

		public static T[] Insert<T> (this T[] arr, T element, int index)
		{
			List<T> output = new List<T>(arr);
			output.Insert(index, element);
			return output.ToArray();
		}

		public static int IndexOf<T> (this Array arr, T element)
		{
			for (int index = 0; index < arr.GetLength(0); index ++)
			{
				if (((T) arr.GetValue(index)).Equals(element))
					return index;
			}
			return -1;
		}

		public static T[] _Sort<T> (this T[] arr, IComparer<T> sorter)
		{
			List<T> output = new List<T>(arr);
			output.Sort(sorter);
			return output.ToArray();
		}

		public static T[] GetHomogenized<T> (T element, uint count)
		{
			T[] output = new T[count];
			for (int i = 0; i < count; i ++)
				output[i] = element;
			return output;
		}

		public static int Count (this IEnumerable enumerable)
		{
			int output = 0;
			IEnumerator enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
				output ++;
			return output;
		}

		public static T Get<T> (this IEnumerable<T> enumerable, int index)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				index --;
				if (index < 0)
					return (T) enumerator.Current;
			}
			return default(T);
		}

		public static float GetMin (this float[] arr)
		{
			float min = arr[0];
			for (int i = 1; i < arr.Length; i ++)
			{
				if (arr[i] < min)
					min = arr[i];
			}
			return min;
		}

		public static float GetMax (this float[] arr)
		{
			float max = arr[0];
			for (int i = 1; i < arr.Length; i ++)
			{
				if (arr[i] > max)
					max = arr[i];
			}
			return max;
		}

		public static List<T> _Add<T> (this List<T> list, T element)
		{
			list.Add(element);
			return list;
		}
		
		public static int Length<T> (this List<T> list)
		{
			return list.Count;
		}
		
		public static List<T> _TrimEnd<T> (this List<T> list, int count)
		{
			list.RemoveRange(list.Count - count, count);
			return list;
		}
		
		public static List<T> _RemoveAt<T> (this List<T> list, int index)
		{
			list.RemoveAt(index);
			return list;
		}
		
		public static List<T> _Remove<T> (this List<T> list, T element)
		{
			list.Remove(element);
			return list;
		}
		
		public static T[] _RemoveAt<T> (this T[] arr, int index)
		{
			arr = arr.RemoveAt(index);
			return arr;
		}
		
		public static T[] _Remove<T> (this T[] arr, T element)
		{
			arr = arr.Remove(element);
			return arr;
		}
		
		public static T[] _Add<T> (this T[] arr, T element)
		{
			arr = arr.Add(element);
			return arr;
		}

		public static T1 GetKey<T1, T2> (this Dictionary<T1, T2> dict, int idx)
		{
			IEnumerator keyEnumerator = dict.Keys.GetEnumerator();
			while (keyEnumerator.MoveNext() && idx > 0)
			{
				if (idx == 0)
					return (T1) keyEnumerator.Current;
				idx --;
			}
			throw new IndexOutOfRangeException();
		}

		public static T1[] GetKeys<T1, T2> (this Dictionary<T1, T2> dict)
		{
			List<T1> output = new List<T1>();
			IEnumerator keyEnumerator = dict.Keys.GetEnumerator();
			while (keyEnumerator.MoveNext())
				output.Add((T1) keyEnumerator.Current);
			return output.ToArray();
		}

		public static T1[] GetKeys<T1, T2> (this SortedDictionary<T1, T2> dict)
		{
			List<T1> output = new List<T1>();
			IEnumerator keyEnumerator = dict.Keys.GetEnumerator();
			while (keyEnumerator.MoveNext())
				output.Add((T1) keyEnumerator.Current);
			return output.ToArray();
		}

		public static bool Contains_IList<T> (this IList<T> list, T element)
		{
			return list.Contains(element);
		}

		public static Vector2 ToVec2 (this float[] components)
		{
			return new Vector2(components[0], components[1]);
		}

		public static Vector3 ToVec3 (this float[] components)
		{
			return new Vector3(components[0], components[1], components[2]);
		}

		public static Vector2 ToVec2 (this int[] components)
		{
			return new Vector2(components[0], components[1]);
		}

		public static Vector3 ToVec3 (this int[] components)
		{
			return new Vector3(components[0], components[1], components[2]);
		}

		public static Vector2Int ToVec2Int (this int[] components)
		{
			return new Vector2Int(components[0], components[1]);
		}

		public static Vector3Int ToVec3Int (this int[] components)
		{
			return new Vector3Int(components[0], components[1], components[2]);
		}

		public static bool ContainsAll<T> (this List<T> list, params T[] values)
		{
			for (int i = 0; i < values.Length; i ++)
			{
				T value = values[i];
				if (!list.Contains(value))
					return false;
			}
			return true;
		}

		public static int OccuranceCount<T> (this List<T> list, T value)
		{
			int output = 0;
			for (int i = 0; i < list.Count; i ++)
			{
				T value2 = list[i];
				if (value2.Equals(value))
					output ++;
			}
			return output;
		}

		public static bool OccurancesHaveSameCount<T> (this List<T> list, params T[] values)
		{
			int occuranceCount = list.OccuranceCount(values[0]);
			for (int i = 1; i < values.Length; i ++)
			{
				T value = values[i];
				if (list.OccuranceCount(value) != occuranceCount)
					return false;
			}
			return true;
		}

		public static bool OccurancesAreAfterOthers<T> (this List<T> list, T[] values, T[] others, bool onlySearchToFirstOccurances = false, bool errorIfLookedForElementNotSeen = false)
		{
			if (values == null)
				throw new ArgumentNullException("'values' is null");
			if (values.Length == 0)
				throw new Exception("'values' is empty");
			if (others == null)
				throw new ArgumentNullException("'others' is null");
			if (others.Length == 0)
				throw new Exception("'others' is empty");
			int maxIndexOfValue = -1;
			for (int i = 0; i < values.Length; i ++)
			{
				T value = values[i];
				bool foundValue = false;
				for (int i2 = 0; i2 < list.Count; i2 ++)
				{
					T value2 = list[i2];
					if (value.Equals(value2))
					{
						foundValue = true;
						if (i2 > maxIndexOfValue)
						{
							if (i2 == list.Count - 1)
								return false;
							maxIndexOfValue = i2;
						}
						if (onlySearchToFirstOccurances)
							break;
					}
				}
				if (!foundValue && errorIfLookedForElementNotSeen)
					throw new Exception("An element in 'values' was looked for but not in 'list'");
			}
			if (maxIndexOfValue == -1)
				throw new Exception("No elements in 'values' are in 'list'");
			bool foundElementInOther = false;
			for (int i = 0; i < others.Length; i ++)
			{
				T other = others[i];
				int indexOfOther = list.IndexOf(other);
				if (indexOfOther != -1)
				{
					if (indexOfOther <= maxIndexOfValue)
						return false;
					foundElementInOther = true;
				}
				else if (errorIfLookedForElementNotSeen)
					throw new Exception("An element in 'others' was looked for but not in 'list'");
			}
			if (!foundElementInOther)
				throw new Exception("No elements in 'others' are in 'list'");
			return true;
		}

		public static int[] IndicesOf<T> (this List<T> list, T value)
		{
			List<int> output = new List<int>();
			int indexOfValue = 0;
			while (true)
			{
				indexOfValue = list.IndexOf(value, indexOfValue);
				if (indexOfValue != -1)
					output.Add(indexOfValue);
				else
					break;
			}
			return output.ToArray();
		}

		public static int[] UniqueOrderedIndicesOf<T> (this List<T> list, params T[] values)
		{
			List<int> output = new List<int>();
			for (int i = 0; i < list.Count; i ++)
			{
				T value = list[i];
				for (int i2 = 0; i2 < values.Length; i2 ++)
				{
					T value2 = values[i2];
					if (value.Equals(value2))
					{
						output.Add(i);
						break;
					}
				}
			}
			return output.ToArray();
		}

		public static int FirstIndexOf<T> (this List<T> list, params T[] values)
		{
			int output = int.MaxValue;
			for (int i = 0; i < list.Count; i ++)
			{
				T value = list[i];
				for (int i2 = 0; i2 < values.Length; i2 ++)
				{
					T value2 = values[i2];
					if (value.Equals(value2) && i < output)
					{
						output = i;
						break;
					}
				}
			}
			if (output == int.MaxValue)
				output = -1;
			return output;
		}

		public static List<T> RemoveCopies<T> (this List<T> list)
		{
			List<T> output = new List<T>(list);
			for (int i = 0; i < output.Count; i ++)
			{
				T element = output[i];
				int lastIndexOfElement;
				do
				{
					lastIndexOfElement = output.LastIndexOf(element);
					if (lastIndexOfElement != i)
						output.RemoveAt(lastIndexOfElement);
				} while (lastIndexOfElement != i);
			}
			return output;
		}

		static void RotateRight (this IList list, int count)
		{
			object element = list[count - 1];
			list.RemoveAt(count - 1);
			list.Insert(0, element);
		}

		public static void RotateRight (this IList list)
		{
			object element = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			list.Insert(0, element);
		}

		public static void RotateLeft (this IList list)
		{
			object element = list[0];
			list.RemoveAt(0);
			list.Add(element);
		}

		public static IEnumerable<IList> Permutations (this IList list, int count)
		{
			if (count == 1)
				yield return list;
			else
			{
				for (int i = 0; i < count; i ++)
				{
					foreach (IList permutation in Permutations(list, count - 1))
						yield return permutation;
					RotateRight (list, count);
				}
			}
		}

		public static IEnumerable<IList> Permutations (this IList list)
		{
			return list.Permutations(list.Count);
		}

		public static IEnumerable<IList> Permutations<T> (this T[] arr, int count)
		{
			return new List<T>(arr).Permutations(count);
		}

		public static IEnumerable<IList> Permutations<T> (this T[] arr)
		{
			return new List<T>(arr).Permutations(arr.Length);
		}

		public static List<List<T>> UniquePermutations<T> (this T[] arr, uint howManyToPick = uint.MaxValue, bool permutationsCanHaveSameElementsWithDifferentOrder = true)
		{
			List<List<T>> output = UniquePermutationsHandler<T>.UniquePermutations(arr, howManyToPick);
			if (!permutationsCanHaveSameElementsWithDifferentOrder)
			{
				for (int i = 0; i < output.Count; i ++)
				{
					List<T> permutation = output[i];
					for (int i2 = i + 1; i2 < output.Count; i2 ++)
					{
						List<T> permutation2 = output[i2];
						if (permutation.Count == permutation2.Count && permutation.ContainsAll(permutation2.ToArray()))
						{
							output.RemoveAt(i2);
							i2 --;
						}
					}
				}
			}
			return output;
		}

		static class UniquePermutationsHandler<T>
		{
			public static List<List<T>> output = new List<List<T>>();

			public static List<List<T>> UniquePermutations (T[] arr, uint howManyToPick = uint.MaxValue)
			{
				output = new List<List<T>>();
				if (howManyToPick == uint.MaxValue)
					howManyToPick = (uint) arr.Length;
				UniquePermutations (arr, new List<T>(), new bool[arr.Length], howManyToPick);
				return output;
			}

			public static void UniquePermutations (T[] arr, List<T> combination, bool[] visited, uint howManyToPick = uint.MaxValue)
			{
				if (combination.Count == howManyToPick)
				{
					output.Add(new List<T>(combination));
					return;
				}
				for (int index = 0; index < arr.Length; ++ index)
				{
					// Check to see if this number has been visited
					if (visited[index])
						continue;
					else if (index > 0 && arr[index].Equals(arr[index - 1]) && !visited[index - 1])
						continue;
					// Set that this index has been visited
					visited[index] = true;
					// Add this number to the combination
					combination.Add(arr[index]);
					// Keep generating permutations
					UniquePermutations (arr, combination, visited, howManyToPick);
					// Unset that this index has been visited
					visited[index] = false;
					// Remove last item as its already been explored
					combination.RemoveAt(combination.Count - 1);
				}
			}
		}
	}
}