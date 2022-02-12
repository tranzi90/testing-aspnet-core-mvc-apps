Feature: TransferToAnotherCustomer
	In order to interact with other customers
	As a registered customer
	I want to transfer my money to other customer

Scenario: Registered customer withdraw from ATM
	Given registered customer Alice with 1000 on account
	And registered customer Bob with 100 on account
	When Alice transfer to Bob amount 100
	Then Alice account balance is 900
	And Bob account balance is 200
