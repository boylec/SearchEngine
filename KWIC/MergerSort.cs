using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWIC
{
    public static class MergerSort
    {
        public static void MergeSort(IComparable[] a)
        {
            var tmp = new IComparable[a.Length];
            MergeSort(a, tmp, 0, a.Length - 1);
        }

        private static void MergeSort(IList<IComparable> a, IList<IComparable> tmp, int left, int right)
        {
            if (left >= right) return;

            var center = (left + right) / 2;
            MergeSort(a, tmp, left, center);
            MergeSort(a, tmp, center + 1, right);
            Merge(a, tmp, left, center + 1, right);
        }

        private static void Merge(IList<IComparable> a, IList<IComparable> tmp, int left, int right, int rightEnd)
        {
            var leftEnd = right - 1;
            var k = left;
            var num = rightEnd - left + 1;

            while (left <= leftEnd && right <= rightEnd)
                if (a[left].CompareTo(a[right]) <= 0)
                    tmp[k++] = a[left++];
                else
                    tmp[k++] = a[right++];

            while (left <= leftEnd)    // Copy rest of first half
                tmp[k++] = a[left++];

            while (right <= rightEnd)  // Copy rest of right half
                tmp[k++] = a[right++];

            // Copy tmp back
            for (var i = 0; i < num; i++, rightEnd--)
                a[rightEnd] = tmp[rightEnd];
        }
    }
}
