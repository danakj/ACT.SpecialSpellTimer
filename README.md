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
<table>
<tr>
<td>&lt;me&gt;</td>
<td>使えます</td>
</tr>

<tr>
<td>&lt;2&gt;～&lt;8&gt;</td>
<td>使えます</td>
</tr>

<tr>
<td>&lt;t&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;tt&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;ft&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;petid&gt;</td>
<td>
FF14が内部のオブジェクトに割当てている一意なIDに置換されます<br />
このIDによってACTが生成している詳細なログにマッチさせると自分のペットだけを識別出来ます<br />
※現在テスト中
</td>
</tr>
</table>
  
3) 俺の歌を聞かせたい    
resources/wav にwaveファイルを投入するとスペスペで使用できるようになります  

テキストコマンド
--------------
FF14の内部からテキストコマンドで一部の機能を制御できます  
/e コマンド  
の書式でコマンドを発行してください  

例) 全てのスペルを無効にする  
/e /spespe changeenabled spells all false  
  
<table>
<tr>
<td>コマンド</td><td>説明</td>
</tr>

<tr>
<td>/spespe refresh spells</td>
<td>スペルリストパネルを一度閉じてリフレッシュする</td>
</tr>

<tr>
<td>/spespe refresh telops</td>
<td>テロップを一度閉じてリフレッシュする</td>
</tr>

<tr>
<td>/spespe refresh me</td>
<td>プレイヤー名のキャッシュを更新する</td>
</tr>

<tr>
<td>/spespe refresh pt</td>
<td>パーティメンバー名のキャッシュを更新する</td>
</tr>

<tr>
<td>/spespe changeenabled spells "サンプルパネル" true</td>
<td>指定したパネルのスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled spells "サンプルスペル" true</td>
<td>指定したスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled telops "サンプルテロップ" true</td>
<td>指定したテロップを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled spells all true</td>
<td>全てのスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled telops all true</td>
<td>全てのテロップを有効にする。falseで無効</td>
</tr>

</table>
  
  
    
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
  
  
お問合せ
--------------
不具合報告、要望、質問及び最新版情報などはTwitterにて  
GitHubと連動しているためツイートは少々五月蠅いかもしれません  
