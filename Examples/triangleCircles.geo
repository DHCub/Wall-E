import "stdlib/mediatrix.geo";
import "stdlib/bisectrix.geo";
	
drawTrig(trig) = let
	a, b, c, _ = trig;
	draw segment(a, b);
	draw segment(a, c);
	draw segment(c, b);
	in 0;

_getUpThirdTrig(eqTrig) = let
	a, b, c, _ = eqTrig;
	ac = segment(a, c);
	med = mediatrix(a, c);
	i1, _ = intersect(med, ac);
	v1 = (b - i1);
	P = i1 + 2*v1/3;
	L = line(P, (P + (a - c)));
	i2, _ = intersect(segment(a, b), L);
	i3, _ = intersect(segment(b, c), L);
	in
	{i2, b, i3};
	
genEqTrig(base) = let
	p1, p2, _ = base;
	m = measure(p1, p2);
	c1 = circle(p1, m);
	c2 = circle(p2, m);
	_, i1, _ = intersect(c1, c2);
	in
	{p1, i1, p2};

drawInCircle(trig) = let
	drawTrig(trig);
	a, b, c, _ = trig;
	bis1 = bisectrix(a, c, b);
	bis2 = bisectrix(a, b, c);
	inC, _ = intersect(bis1, bis2);
	p1, _ = intersect(bis1, segment(a, b));
	r = measure(inC, p1);
	draw circle(inC, r);
	in 0;
	
triangleCircles(n, trig) = if n == 1 then 1 else let
	a, b, c, _ = trig;
	drawInCircle(trig);
	shiftTrig(trig) = let
		a, b, c, _ = trig;
		in {b, c, a};
	t1 = _getUpThirdTrig(trig);
	_T1 = shiftTrig(trig);
	t2 = _getUpThirdTrig(_T1);
	_T2 = shiftTrig(_T1);
	t3 = _getUpThirdTrig(_T2);
	triangleCircles(n - 1, t1);
	triangleCircles(n - 1, t2);
	triangleCircles(n - 1, t3);
	in 0;

trig = genEqTrig({point(-3, -3), point(3, -3)});
drawTrig(trig);
triangleCircles(6, trig);
