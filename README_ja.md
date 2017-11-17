# gh-random-generator

## About
- Grasshopper 用のメルセンヌ・ツイスタです．
- 線形合同法を使用する Grasshopper 標準の Random コンポーネントに比べ，高品質な乱数が生成できます．
- ghuser オブジェクトを[こちら](https://github.com/nikkenddl/gh-random-generator/tree/master/ghuser)からダウンロードできます．

### Mersenne Twister

- [MTのオリジナル](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/mt.html).

### 利用

![LCG and Mersenne Twister](https://raw.githubusercontent.com/nikkenddl/gh-random-generator/master/docs/images/LCG-MT-Comparison.png)

![Screenshot](https://github.com/nikkenddl/gh-random-generator/blob/master/docs/images/MT19937_usage.PNG?raw=true)

- Grasshopper の random コンポーネントと同様に使用できます．
- メルセンヌ・ツイスタを使用すると，3次元以上に均一に分布する点をすばやく作れます．
- 同梱の Box-Muller コンポーネントを使用して，正規分布を作成することができます．
- 同梱の Power Law コンポーネントを使用して，べき乗分布を作成することができます．

## 既知の問題

- これは単に Grasshopper の C# コンポーネントを利用して作成しているので，速度が必要な場面には向きません．
