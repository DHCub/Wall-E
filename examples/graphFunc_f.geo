plot(poly) = let
	a, b, r1 = poly;
	draw segment(a, b);
	in
		if !r1 then 1 else let
		_, r = poly;
		plot(r);
		in 1;

abs(x) = (x^2)^0.5;
graph(x1, x2) = 
  if abs(x1 - x2) <= 0.1 then {point(x1, f(x1))}
  else graph(x1, (x1 + x2)/2) + graph((x1 + x2)/2, x2);

plot(graph(-20, 20));
