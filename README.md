# gh-random-generator

## About
- Mersenne Twister for Grasshopper.
- Better pseudorandom generator compared to Grasshopper's original "Random" component which uses LCG.
- Download Binary [Here](https://github.com/nikkenddl/gh-random-generator/tree/master/ghuser)

### Mersenne Twister

- [Original Authors' webpage](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/mt.html).

## Usage

- Same usage as GH's Random component.
- Box-Muller component can generate 2 normally-distributed random numbers.

![Screenshot](https://github.com/nikkenddl/gh-random-generator/blob/master/docs/images/MT19937_usage.PNG?raw=true)

## Known Issues

- Due to implementation naively using C# script component, this is relatively slow compared to GH's Random component.
