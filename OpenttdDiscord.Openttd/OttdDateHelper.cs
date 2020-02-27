using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    internal class OttdDateHelper
    {
        internal static readonly List<ushort> monthDateFromYear = new List<ushort>();

        internal static readonly ushort[] daysTillMonth = new ushort[12];

        internal const uint daysInYear = 365; // days per year
        internal const uint daysInLeapYear = 366; // sometimes, you need one day more...




        static OttdDateHelper()
        {
            static int M(int a, int b) => (a << 5) | b;

            int[] array = new int[]
            {
                M( 0, 1), M( 0, 2), M( 0, 3), M( 0, 4), M( 0, 5), M( 0, 6), M( 0, 7), M( 0, 8), M( 0, 9), M( 0, 10), M( 0, 11), M( 0, 12), M( 0, 13), M( 0, 14), M( 0, 15), M( 0, 16), M( 0, 17), M( 0, 18), M( 0, 19), M( 0, 20), M( 0, 21), M( 0, 22), M( 0, 23), M( 0, 24), M( 0, 25), M( 0, 26), M( 0, 27), M( 0, 28), M( 0, 29), M( 0, 30), M( 0, 31),
                M( 1, 1), M( 1, 2), M( 1, 3), M( 1, 4), M( 1, 5), M( 1, 6), M( 1, 7), M( 1, 8), M( 1, 9), M( 1, 10), M( 1, 11), M( 1, 12), M( 1, 13), M( 1, 14), M( 1, 15), M( 1, 16), M( 1, 17), M( 1, 18), M( 1, 19), M( 1, 20), M( 1, 21), M( 1, 22), M( 1, 23), M( 1, 24), M( 1, 25), M( 1, 26), M( 1, 27), M( 1, 28), M( 1, 29),
                M( 2, 1), M( 2, 2), M( 2, 3), M( 2, 4), M( 2, 5), M( 2, 6), M( 2, 7), M( 2, 8), M( 2, 9), M( 2, 10), M( 2, 11), M( 2, 12), M( 2, 13), M( 2, 14), M( 2, 15), M( 2, 16), M( 2, 17), M( 2, 18), M( 2, 19), M( 2, 20), M( 2, 21), M( 2, 22), M( 2, 23), M( 2, 24), M( 2, 25), M( 2, 26), M( 2, 27), M( 2, 28), M( 2, 29), M( 2, 30), M( 2, 31),
                M( 3, 1), M( 3, 2), M( 3, 3), M( 3, 4), M( 3, 5), M( 3, 6), M( 3, 7), M( 3, 8), M( 3, 9), M( 3, 10), M( 3, 11), M( 3, 12), M( 3, 13), M( 3, 14), M( 3, 15), M( 3, 16), M( 3, 17), M( 3, 18), M( 3, 19), M( 3, 20), M( 3, 21), M( 3, 22), M( 3, 23), M( 3, 24), M( 3, 25), M( 3, 26), M( 3, 27), M( 3, 28), M( 3, 29), M( 3, 30),
                M( 4, 1), M( 4, 2), M( 4, 3), M( 4, 4), M( 4, 5), M( 4, 6), M( 4, 7), M( 4, 8), M( 4, 9), M( 4, 10), M( 4, 11), M( 4, 12), M( 4, 13), M( 4, 14), M( 4, 15), M( 4, 16), M( 4, 17), M( 4, 18), M( 4, 19), M( 4, 20), M( 4, 21), M( 4, 22), M( 4, 23), M( 4, 24), M( 4, 25), M( 4, 26), M( 4, 27), M( 4, 28), M( 4, 29), M( 4, 30), M( 4, 31),
                M( 5, 1), M( 5, 2), M( 5, 3), M( 5, 4), M( 5, 5), M( 5, 6), M( 5, 7), M( 5, 8), M( 5, 9), M( 5, 10), M( 5, 11), M( 5, 12), M( 5, 13), M( 5, 14), M( 5, 15), M( 5, 16), M( 5, 17), M( 5, 18), M( 5, 19), M( 5, 20), M( 5, 21), M( 5, 22), M( 5, 23), M( 5, 24), M( 5, 25), M( 5, 26), M( 5, 27), M( 5, 28), M( 5, 29), M( 5, 30),
                M( 6, 1), M( 6, 2), M( 6, 3), M( 6, 4), M( 6, 5), M( 6, 6), M( 6, 7), M( 6, 8), M( 6, 9), M( 6, 10), M( 6, 11), M( 6, 12), M( 6, 13), M( 6, 14), M( 6, 15), M( 6, 16), M( 6, 17), M( 6, 18), M( 6, 19), M( 6, 20), M( 6, 21), M( 6, 22), M( 6, 23), M( 6, 24), M( 6, 25), M( 6, 26), M( 6, 27), M( 6, 28), M( 6, 29), M( 6, 30), M( 6, 31),
                M( 7, 1), M( 7, 2), M( 7, 3), M( 7, 4), M( 7, 5), M( 7, 6), M( 7, 7), M( 7, 8), M( 7, 9), M( 7, 10), M( 7, 11), M( 7, 12), M( 7, 13), M( 7, 14), M( 7, 15), M( 7, 16), M( 7, 17), M( 7, 18), M( 7, 19), M( 7, 20), M( 7, 21), M( 7, 22), M( 7, 23), M( 7, 24), M( 7, 25), M( 7, 26), M( 7, 27), M( 7, 28), M( 7, 29), M( 7, 30), M( 7, 31),
                M( 8, 1), M( 8, 2), M( 8, 3), M( 8, 4), M( 8, 5), M( 8, 6), M( 8, 7), M( 8, 8), M( 8, 9), M( 8, 10), M( 8, 11), M( 8, 12), M( 8, 13), M( 8, 14), M( 8, 15), M( 8, 16), M( 8, 17), M( 8, 18), M( 8, 19), M( 8, 20), M( 8, 21), M( 8, 22), M( 8, 23), M( 8, 24), M( 8, 25), M( 8, 26), M( 8, 27), M( 8, 28), M( 8, 29), M( 8, 30),
                M( 9, 1), M( 9, 2), M( 9, 3), M( 9, 4), M( 9, 5), M( 9, 6), M( 9, 7), M( 9, 8), M( 9, 9), M( 9, 10), M( 9, 11), M( 9, 12), M( 9, 13), M( 9, 14), M( 9, 15), M( 9, 16), M( 9, 17), M( 9, 18), M( 9, 19), M( 9, 20), M( 9, 21), M( 9, 22), M( 9, 23), M( 9, 24), M( 9, 25), M( 9, 26), M( 9, 27), M( 9, 28), M( 9, 29), M( 9, 30), M( 9, 31),
                M(10, 1), M(10, 2), M(10, 3), M(10, 4), M(10, 5), M(10, 6), M(10, 7), M(10, 8), M(10, 9), M(10, 10), M(10, 11), M(10, 12), M(10, 13), M(10, 14), M(10, 15), M(10, 16), M(10, 17), M(10, 18), M(10, 19), M(10, 20), M(10, 21), M(10, 22), M(10, 23), M(10, 24), M(10, 25), M(10, 26), M(10, 27), M(10, 28), M(10, 29), M(10, 30),
                M(11, 1), M(11, 2), M(11, 3), M(11, 4), M(11, 5), M(11, 6), M(11, 7), M(11, 8), M(11, 9), M(11, 10), M(11, 11), M(11, 12), M(11, 13), M(11, 14), M(11, 15), M(11, 16), M(11, 17), M(11, 18), M(11, 19), M(11, 20), M(11, 21), M(11, 22), M(11, 23), M(11, 24), M(11, 25), M(11, 26), M(11, 27), M(11, 28), M(11, 29), M(11, 30), M(11, 31),
            };

            foreach (int x in array)
                monthDateFromYear.Add((ushort)x);

            daysTillMonth[0] = 0;
            daysTillMonth[1] = 31;
            daysTillMonth[2] = (ushort)(daysTillMonth[1] + 29);
            daysTillMonth[3] = (ushort)(daysTillMonth[2] + 31);
            daysTillMonth[4] = (ushort)(daysTillMonth[3] + 30);
            daysTillMonth[5] = (ushort)(daysTillMonth[4] + 31);
            daysTillMonth[6] = (ushort)(daysTillMonth[5] + 30);
            daysTillMonth[7] = (ushort)(daysTillMonth[6] + 31);
            daysTillMonth[8] = (ushort)(daysTillMonth[7] + 31);
            daysTillMonth[9] = (ushort)(daysTillMonth[8] + 30);
            daysTillMonth[10] = (ushort)(daysTillMonth[9] + 31);
            daysTillMonth[11] = (ushort)(daysTillMonth[10] + 30);
        }

        static internal bool IsLeapYear(uint year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

        static internal uint DaysInYear(uint year) => IsLeapYear(year) ? daysInLeapYear : daysInYear;
    }
}
