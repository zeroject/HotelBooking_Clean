Feature: Hotel Booking

  Scenario: Create a booking with the first and second rooms
    Given the first room is 101
    And the second room is 102
    When I create a booking with "2024-10-23,2024-11-23"
    Then booking is created with 2024-11-24,2024-11-30 the result is true

  Scenario: Attempt to create a booking without rooms
    Given the first room is 101
    And the second room is 102
    When I create a booking with "2024-10-24,2024-10-30"
    Then booking is created with 2024-11-10,2024-11-24 the result is false