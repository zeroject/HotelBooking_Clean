using HotelBooking.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Specs.StepDefinitions
{
    [Binding]
    public class CreatBookingStepDefinition
    {
        private readonly IRepository<Booking> bookingRepository;
        private readonly IRepository<Room> roomRepository;

        BookingManager bookingManager;

        public CreatBookingStepDefinition(IRepository<Booking> bookingRepository, IRepository<Room> roomRepository)
        {
            this.bookingRepository = bookingRepository;
            this.roomRepository = roomRepository;
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [StepArgumentTransformation]
        public Booking TransformBooking(string bookingDetails)
        {
            // Transform the booking details into a Booking object
            Booking booking = new Booking();
            booking.StartDate = DateTime.Parse(bookingDetails.Split(',')[0]);
            booking.EndDate = DateTime.Parse(bookingDetails.Split(",")[1]);
            booking.IsActive = true;
            return booking;
        }

        [Given("the first room is {int}")]
        public void GivenTheFirstRoomIS(int id)
        {
            Room room = new Room();
            room.Id = id;
            roomRepository.Add(room);
        }

        [Given("the second room is {int}")]
        public void GivenTheSecondRoomIS(int id)
        {
            Room room = new Room();
            room.Id = id;
            roomRepository.Add(room);
        }

        [Then("booking is created with (.*) the result is (.*)")]
        public void ThenTheBookingISCreatedItShouldReturn(Booking booking, bool result)
        {
            bool v = bookingManager.CreateBooking(booking);
            if (result == v) { };
        }
    }
}
