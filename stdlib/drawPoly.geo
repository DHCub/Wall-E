getLast(seq) = let
	a, r = seq;
	in
	if !r then a
	else getLast(r);

_drawPoly(poly) = let
	a, b, r1 = poly;
	draw segment(a, b);
	in
		if !r1 then 1 else let
		_, r = poly;
		_drawPoly(r);
		in 1;

drawPoly(poly) = let 
	_drawPoly(poly);
	a, _ = poly;
	draw segment(a, getLast(poly));
	in 1;
