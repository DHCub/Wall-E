bisectrix(a, c, b) = let
	m = measure(c, a);
	b2, _ = intersect(circle(c, m), ray(c, b));
	i1, i2, _ = intersect(circle(a, m), circle(b2, m));
	in
	line(i1, i2);
