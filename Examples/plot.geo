plot(poly) = 
let
	prev = poly[0];

	for (i = 1; i < count(poly); i++)
	let
		draw segment(prev, poly[i]);
		prev := poly[i];
	in 0;
in
	0;
