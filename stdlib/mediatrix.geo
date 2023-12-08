mediatrix(p1, p2) = 
    let
		l1 = line(p1, p2);
		m = measure (p1, p2);
		c1 = circle (p1, m);
		c2 = circle (p2, m);
		i1,i2,_ = intersect(c1, c2);
		l2 = line(i1, i2);
    in l2;
