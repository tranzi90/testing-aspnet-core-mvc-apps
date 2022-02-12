Feature: WithdrawFromAtm
	In order to have cash
	As a registered customer
	I want to withdraw my money from ATM

Scenario: Registered customer withdraw from ATM
	Given registered customer Alice with 1000 on account
	And registered ATM with 1000 balance
	When Alice withdraw from ATM 1000
	Then Alice cash is 1000
	And Alice account balance is 0
	And ATM balance is 0
