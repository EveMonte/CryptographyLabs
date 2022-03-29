// library used to emulate uint64_t numbers
var Long  = require('long');

//Blake2b IV array
var BLAKE2B_IV = [
	new Long(0x6a09e667f3bcc908),
	new Long(0xbb67ae8584caa73b),
	new Long(0x3c6ef372fe94f82b),
	new Long(0xa54ff53a5f1d36f1),
	new Long(0x510e527fade682d1),
	new Long(0x9b05688c2b3e6c1f),
	new Long(0x1f83d9abfb41bd6b),
	new Long(0x5be0cd19137e2179),
]

//Blake2b rotation
function rotr64(w,c){

	var a = w.shr_u(c);
	var b = w.shr_u(64 - c);

	return a.or(b);
}

//Blake2b's G function
//it appears that r and i arent used
function blake2bG(r,i,a,b,c,d){
	a = a.add(b);
	d = rotr64(d.xor(a), 32);
	c = c.add(d);
	b = rotr64(b.xor(c), 24);
	a = a.add(b);
	d = rotr64(d.xor(a), 16);
	c = c.add(d);
	d = rotr64(b.xor(c), 63);
}

//One Round of the Blake2b's compression function
//it appears that r and i arent used
function roundLyra(r){
	G(r,0,v[ 0],v[ 4],v[ 8],v[12]);
    G(r,1,v[ 1],v[ 5],v[ 9],v[13]);
    G(r,2,v[ 2],v[ 6],v[10],v[14]);
    G(r,3,v[ 3],v[ 7],v[11],v[15]);
    G(r,4,v[ 0],v[ 5],v[10],v[15]);
    G(r,5,v[ 1],v[ 6],v[11],v[12]); 
    G(r,6,v[ 2],v[ 7],v[ 8],v[13]); 
    G(r,7,v[ 3],v[ 4],v[ 9],v[14]);
}
