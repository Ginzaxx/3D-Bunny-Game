# 🐰 Carrot Catcher Game - Unity Setup Guide

## Overview
Game 3D catching game di mana Kelinci menangkap wortel yang jatuh.
Terinspirasi mekanik Pou food drop.

---

## 📁 Daftar Script

| Script | Attach ke | Keterangan |
|--------|-----------|-----------|
| `GameManager.cs` | GameObject "GameManager" | State machine utama |
| `PlayerController.cs` | Prefab Kelinci | Gerak kiri/kanan + tangkap |
| `FallingObject.cs` | Prefab Wortel & Rubah | Logika jatuh & efek |
| `FoxClickDetector.cs` | Prefab Rubah (tambahan) | Countdown click rubah |
| `SpawnManager.cs` | GameObject "SpawnManager" | Spawn wortel & rubah |
| `TimerManager.cs` | GameObject "TimerManager" | Countdown timer |
| `ScoreManager.cs` | GameObject "ScoreManager" | Skor & high score |
| `WeatherManager.cs` | GameObject "WeatherManager" | Sistem cuaca |
| `CameraController.cs` | Main Camera | Angle miring 45° |
| `UIManager.cs` | GameObject "UIManager" | Semua panel UI |
| `AudioManager.cs` | GameObject "AudioManager" | BGM & SFX |

---

## 🏗️ Hierarchy Scene

```
Scene
├── GameManager
├── AudioManager
├── SpawnManager
├── TimerManager
├── ScoreManager
├── WeatherManager
├── UIManager
│   ├── Canvas
│   │   ├── MenuPanel
│   │   ├── HUDPanel
│   │   ├── PausePanel
│   │   └── GameOverPanel
│
├── Main Camera (+ CameraController)
│
├── Environment
│   ├── Ground (tag: "Ground")
│   ├── Trees (Brokoli, Jagung, Terong, Sawit)
│   ├── Pohon Carrot (dekorasi)
│   └── Awan x3
│
├── Player_Kelinci (+ PlayerController, Rigidbody)
│   └── Basket (tag: "Basket", BoxCollider trigger)
│
└── Lighting
    └── Directional Light
```

---

## 🥕 Setup Prefabs

### Wortel Normal (CarrotNormal.prefab)
- Tambahkan komponen: `FallingObject`
- Set `objectType` = CarrotNormal
- Set `timerBonus` = 5
- Warna: Kuning/Orange
- Tambahkan: Rigidbody (Is Kinematic = true), Collider

### Wortel Beku (CarrotFrozen.prefab)  
- Tambahkan komponen: `FallingObject`
- Set `objectType` = CarrotFrozen
- Set `timerPenalty` = 3
- Warna: Biru
- Tambahkan: Rigidbody (Is Kinematic = true), Collider

### Rubah (Fox.prefab)
- Tambahkan komponen: `FallingObject` + `FoxClickDetector`
- Set `objectType` = Fox
- Set `foxClickWindow` = 2
- Tambahkan World Space Canvas dengan countdown timer UI
- Tambahkan: Collider untuk raycast detection

---

## ⚙️ Setup Player

1. Import model Kelinci 3D
2. Attach `PlayerController.cs`
3. Assign `basketTransform` ke child object "Basket"
4. Basket object: tambahkan BoxCollider (Is Trigger = true), tag "Basket"
5. Assign Animator jika ada animasi
6. Set boundary X sesuai lebar arena

---

## 📷 Camera Setup

CameraController settings untuk tampilan miring:
```
gameplayOffset: (0, 8, -6)
gameplayRotation: (45, 0, 0)
```
Sesuaikan Y dan Z untuk zoom in/out.

---

## 🌤️ Cuaca & Efek

### Siang Kering (DayDry) - Default
- Normal spawn rate
- Rubah muncul tiap ~8 detik

### Sore Kering (AfternoonDry)
- Rubah muncul lebih sering (tiap ~4 detik)
- Warna lighting oranye/kemerahan

### Salju (Snow)
- Kecepatan player berkurang 40%
- Kecepatan jatuh wortel berkurang 30%
- Wortel Beku spawn lebih sering (40% chance)
- Aktifkan Snow Particle System

---

## 🎮 Tags yang Diperlukan

Buat tags berikut di Unity (Edit > Project Settings > Tags):
- `Player`
- `Basket`
- `Ground`
- `Carrot`
- `Fox`

---

## 🔗 Menghubungkan Manager di Inspector

Di GameManager, assign semua referensi:
- timerManager → GameObject TimerManager
- spawnManager → GameObject SpawnManager  
- scoreManager → GameObject ScoreManager
- weatherManager → GameObject WeatherManager
- uiManager → GameObject UIManager
- cameraController → Main Camera

---

## 📱 Mobile Support

- PlayerController sudah support touch input
- FoxClickDetector sudah support touch tap
- Pastikan Canvas Scaler di-set ke "Scale With Screen Size"
- Reference Resolution: 1080 x 1920 (portrait) atau 1920 x 1080 (landscape)

---

## ✅ Checklist Setup

- [v] Semua Manager objects dibuat di scene
- [v] Prefabs Wortel Normal, Wortel Beku, Rubah dibuat
- [v] Tags "Basket" dan "Ground" diassign
- [v] SpawnManager.carrotNormalPrefab diassign
- [v] SpawnManager.carrotFrozenPrefab diassign  
- [v] SpawnManager.foxPrefab diassign
- [v] TimerManager.startTime diset (default: 60 detik)
- [ ] CameraController rotation diset ke (45, 0, 0)
- [ ] UI Canvas panels dibuat dan diassign ke UIManager
- [ ] Audio clips diassign ke AudioManager (opsional)
