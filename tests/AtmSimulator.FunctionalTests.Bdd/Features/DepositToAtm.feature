Feature: DepositToAtm
	In order to have money on my account
	As a registered customer
	I want to deposit my money to ATM

Scenario: Registered customer deposit to ATM
	Given registered customer Alice with 1000 cash
	And registered ATM with 0 balance
	When Alice deposit to ATM 1000
	Then Alice cash is 0
	And Alice account balance is 1000
	And ATM balance is 1000
