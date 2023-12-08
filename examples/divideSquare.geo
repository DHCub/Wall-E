import "stdlib/mediatrix.geo";
import "stdlib/drawPoly.geo";


getSquare(a, b) = let
	med = mediatrix(a, b);
	i1, _ = intersect(med, segment(a, b));
	h, _ = intersect(circle(i1, measure(a, b)), med);
	c = h + (b - a)/2;
	d = h + (a - b)/2;
	in
	{a, b, c, d};

frac(n, quad) = if !n then 1 else let
	a, b, c, d, _ = quad;
	aPrime, _ = intersect(mediatrix(a, b), segment(a, b));
	dPrime = aPrime + (c - b);
	draw segment(aPrime, dPrime);
	in frac(n - 1, {b, c, dPrime, aPrime});
	
p1 = point(3, 3);
p2 = point(-3, 3);
quad = getSquare(p1, p2);
drawPoly(quad);
frac(20, quad);
	
