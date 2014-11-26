ACT.SpecialSpellTimer
=====================

概要
-------------
見やすさを改善した特別なスペルタイマーを提供します  
愛称は「スペスペ」  
  
![sample](https://raw.githubusercontent.com/anoyetta/ACT.SpecialSpellTimer/master/sample.png)  

使い方
--------------
resources  
ACT.SpecialSpellTimer.dll  
をACTのインストールディレクトリにコピーします  
その後、プラグインとしてACT.SpecialSpellTimer.dllを追加してください  
  
1) DoTの開始にヒットさせてDoT継続時間を可視化したい  
[エフェクトを受けた人の名前] gains the effect of フラクチャー from [エフェクトを与えた人の名前]  
ACTが吐き出すログには上記のような独自のログがあります  
これに対して正規表現を設定して、自身が与えたDoT（その他デバフも可）の開始を検出してください  
  
2) ゲーム内のプレースホルダは使えないの？  
一部は使えるように対応しています  
<pre>
&lt;me&gt;     → 使えます  
&lt;2&gt;～&lt;8&gt; → プレリリース中  
&lt;t&gt;      → 負荷が高いため搭載できません  
&lt;tt&gt;     → 負荷が高いため搭載できません  
&lt;ft&gt;     → 負荷が高いため搭載できません  
</pre>  
  
3) 俺の歌を聞かせたい    
resources/wav にwaveファイルを投入するとスペスペで使用できるようになります  
  
    
最新リリース
--------------
**[こちらからダウンロードしてください](https://github.com/anoyetta/ACT.SpecialSpellTimer/releases/latest)**  
  
  
ライセンス
--------------
三条項BSDライセンス  
Copryright (c) 2014, anoyetta  
https://github.com/anoyetta/ACT.SpecialSpellTimer/blob/master/LICENSE  
  
  
謝辞
--------------
・GB19xx様  
https://github.com/GB19xx/ACT.TPMonitor  
のFF14ヘルパークラスを流用させていただきました  

・魔王魂様  
http://maoudamashii.jokersounds.com/  
音楽素材といったら魔王魂。  
同梱されたwaveサウンドファイルの著作権は魔王魂に帰属します  
