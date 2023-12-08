getLast(seq) = let
	a, r = seq;
	in
	if !r then a
	else getLast(r);
