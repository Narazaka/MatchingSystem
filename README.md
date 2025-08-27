# Matching System

ランダムに1対1個室マッチングするワールドシステム

## Install

### VCC用インストーラーunitypackageによる方法（おすすめ）

https://github.com/Narazaka/MatchingSystem/releases/latest から `net.narazaka.vrchat.matching-system-installer.zip` をダウンロードして解凍し、対象のプロジェクトにインポートする。

### VCCによる方法

1. https://vpm.narazaka.net/ から「Add to VCC」ボタンを押してリポジトリをVCCにインストールします。
2. VCCでSettings→Packages→Installed Repositoriesの一覧中で「Narazaka VPM Listing」にチェックが付いていることを確認します。
3. アバタープロジェクトの「Manage Project」から「Matching System」をインストールします。

## Usage

### 前提

まずTextMesh Pro VRC Fallback Font JPの設定をする必要があります。

1. Project SettingsのTextMesh Pro→Import TMP Essentialsを実行
2. メニューのTools→TextMesh Pro VRC Fallback Font JPを設定 を実行

### 構築済みPrefabを利用

#### 最小限の構成

1. 新しいシーンを作り、MainCameraを削除する
2. `MatchingSystemMinimalSet` プレハブを配置する

#### ベーシックな構成

0. 必要パッケージをVCCから追加でインストールする
  - Quiz System
  - VRC Player Tag Marker
1. 新しいシーンを作り、MainCameraを削除する
2. `MatchingSystemBasicSet` プレハブを配置する
3. MatchingSystemBasicSet/EntryRoom/Guardにある、QuizSystemとTagMarkerを設定する。

### カスタマイズ

#### 時間を調整

- `SessionTimeSelector` プレハブのCanvas/Panel/Options配下にあるボタンを設定する
  - 標準外の時間を設定する場合、Set Session TimeoutコンポーネントのSession Timeout値を調整し、子のテキストも書き換える。

### いちから構築する場合

#### 最小限の構成

1. スポーン地点を作る
  - VRC Scene Descriptor（VRCWorld）を配置する
  - VRC Scene Descriptor のReference Cameraを設定し、カメラのClipping PlanesのFar値を小さくする（スポーン地点及びマッチング部屋の大きさを見渡せるのに十分な距離を担保して）
    - （オプショナル）同様にカメラのClipping PlanesのNear値を小さくする（所謂ガチ恋距離が出来るようになります）
2. スポーン地点全体を囲うように `SpawnAreaCollider` プレハブをスケールして配置する
3. スポーン地点に `JoinButton` プレハブを配置する（セッションに入るボタン）
4. （オプショナル）スポーン地点に `SessionTimeSelector` プレハブを配置する（時間指定UI）
5. スポーン地点とは別の場所に、`MatchingSystem` プレハブを配置する
6. `MatchingSystem/MatchingManager` の `Area Partition Generator` で、
  - Room Settings で `(ワールド収容人数 / 2) + 1` と同数のルームを作る（Room Countを調整する）
  - Room Prefabは `MatchingRoomn` プレハブを継承してカスタマイズしたものを使うとよいです。
    - Modelsに部屋のモデルを入れる
    - System/SpawnPointsでマッチングする人のスポーン地点を動かす
    - System/ControlPositionでコントロールUIのスポーン地点を動かす
    - System/InformationPositionで情報UIのスポーン地点を動かす
6. `PlayerObjectPool` オブジェクトが出来ているので、`Pool Size`を `ワールド収容人数 + 2` にする

#### スポーン地点を分散し入場チェックをかける構成

1. VRC Scene Descriptor（VRCWorld）を設定する
  - VRC Scene Descriptor（VRCWorld）を配置する
  - VRC Scene Descriptor のReference Cameraを設定し、カメラのClipping PlanesのFar値を小さくする（スポーン地点及びマッチング部屋の大きさを見渡せるのに十分な距離を担保して）
    - （オプショナル）同様にカメラのClipping PlanesのNear値を小さくする（所謂ガチ恋距離が出来るようになります）
2. Entry Roomパッケージの `EntryRoom` プレハブを配置する
3. `Area Partition Generator` で、
  - Room Settings で `ワールド収容人数 + 2` と同数のルームを作る
4. 生成されたスポーン地点全体を囲うように `SpawnAreaCollider` プレハブをスケールして配置する
5. Succeedオブジェクト配下に `JoinButton` プレハブを配置する（セッションに入るボタン）
6. Guardオブジェクト配下にスポーン地点に配置するオブジェクトを追加する
  - 入室チェック（クイズ等）を設定し、クリア時にSucceedオブジェクトをActiveにするように設定する。
  - （オプショナル） `SessionTimeSelector` プレハブを配置する（時間指定UI）
7. 以下は最小限の構成の「スポーン地点とは別の場所に、`MatchingSystem` プレハブを配置する」と同様

## Changelog

- 0.1.0-alpha.0: とりあえず

## License

[Zlib License](LICENSE.txt)
