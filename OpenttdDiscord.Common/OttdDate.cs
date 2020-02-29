using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
	public class OttdDate
	{
		public byte Day { get; }
		public byte Month { get; }

		public uint Year { get; }

		public OttdDate(byte day, byte month, uint year)
		{
			this.Day = day;
			this.Month = month;
			this.Year = year;
		}

		public OttdDate(uint date)
		{
			/* Year determination in multiple steps to account for leap
			 * years. First do the large steps, then the smaller ones.
			 */

			/* There are 97 leap years in 400 years */
			uint year = (uint)(400 * (date / (OttdDateHelper.daysInYear * 400 + 97)));
			uint rem = (uint)(date % (OttdDateHelper.daysInYear * 400 + 97));
			ushort x;

			if (rem >= OttdDateHelper.daysInYear * 100 + 25)
			{
				/* There are 25 leap years in the first 100 years after
				 * every 400th year, as every 400th year is a leap year */
				year += 100;
				rem -= OttdDateHelper.daysInYear * 100u + 25u;

				/* There are 24 leap years in the next couple of 100 years */
				year += 100 * (rem / (OttdDateHelper.daysInYear * 100 + 24));
				rem = (rem % (OttdDateHelper.daysInYear * 100 + 24));
			}

			if (!OttdDateHelper.IsLeapYear(year) && rem >= OttdDateHelper.daysInYear * 4)
			{
				/* The first 4 year of the century are not always a leap year */
				year += 4;
				rem -= OttdDateHelper.daysInYear * 4;
			}

			/* There is 1 leap year every 4 years */
			year += 4 * (rem / (OttdDateHelper.daysInYear * 4 + 1));
			rem = rem % (OttdDateHelper.daysInYear * 4 + 1);

			/* The last (max 3) years to account for; the first one
			 * can be, but is not necessarily a leap year */
			while (rem >= OttdDateHelper.DaysInYear(year))
			{
				rem -= OttdDateHelper.DaysInYear(year);
				year++;
			}

			/* Skip the 29th of February in non-leap years */
			if (!OttdDateHelper.IsLeapYear(year) && rem >= OttdDateHelper.daysTillMonth[2] - 1) rem++;

			this.Year = year;
			x = OttdDateHelper.monthDateFromYear[(int)rem];
			this.Month = (byte)(x >> 5);
			this.Day = (byte)(x & 0x1F);
		}

		public override string ToString() => $"{Year}-{Month}-{Day}";
	}
}
