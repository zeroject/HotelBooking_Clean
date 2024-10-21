Feature: Hotel Booking

  Scenario: Create a booking with the first and second rooms
    Given the first room is 101
    And the second room is 102
    When I create a booking with "08-10-2024,22-10-2024"
    Then booking is created with "08-12-2024,22-10-2024" the result is true

  Scenario: Attempt to create a booking without rooms
    Given the first room is 101
    And the second room is 102
    When I create a booking with "08-10-2024,22-10-2024"
    Then booking is created with "08-10-2024,22-10-2024" the result is false