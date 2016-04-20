select * from Attempts where Source = 0 and Direction = 0 and Type = 3 and (
		AttemptNumber = 10 or AttemptNumber = 28 or AttemptNumber = 46 or AttemptNumber = 64 or AttemptNumber = 82 or AttemptNumber = 100 or AttemptNumber = 118 or AttemptNumber = 136)
		order by ID;