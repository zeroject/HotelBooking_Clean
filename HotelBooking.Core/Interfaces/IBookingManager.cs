using System;
using System.Collections.Generic;

namespace HotelBooking.Core
{
    public interface IBookingManager
    {
        /// <summary>
        /// Creates a booking and returns true if successful and false if not.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        bool CreateBooking(Booking booking);

        /// <summary>
        /// Gets a room that is available for booking on the given dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        int FindAvailableRoom(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets a list of dates that are fully occupied between the given dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<DateTime> GetFullyOccupiedDates(DateTime startDate, DateTime endDate);
    }
}
