# 🌌 Arman’s Journey

**Educational Web3 Adventure Game on Solana**  
_An interactive RPG where learning, exploration, and ownership meet blockchain technology._

---

## 🏆 Submission to 2025 Solana Colosseum by:

- **Issatay Tokpakbayev** — Game Designer, Rust Developer. [GitHub](https://github.com/kusetos) | [TG](https://t.me/kusetos) | [LinkedIn]([https://t.me/kusetos](https://www.linkedin.com/in/issatay-tokpakbayev-305b86280/))
      

---

## 🔗 Resources

- 🎞️ [Presentation Deck](https://www.youtube.com/watch?v=EyvItFC25UY)
    
- 🎮 [Gameplay Video Demo](https://www.youtube.com/watch?v=RKIPxgAKr5Q)
    
- 🌐 [Live Demo (Web Build)](https://idosgames.com/en/app/?id=7K6XJHUK)
                

---

## ❗ Problem and Solution

### 🎯 The Problem

Most Web3 games focus on token speculation instead of **fun, pazzle gameplay**.  
Players rarely **own their achievements or assets**, and blockchain complexity discourages casual gamers.

### 💡 Our Solution

**The Sin Of The Fallen Angel** redefines blockchain gaming through:

1. **True Player Ownership** — all earned items, artifacts, and achievements are minted as NFTs on Solana.
    
2. **Educational Gameplay** — quests and puzzles help players learn real-world knowledge.
        
3. **Earn and Trade** — items and collectibles can be freely exchanged or traded between players.
    

---

## 🎮 Summary of Submission Features

- ✅ Built on **Idos Game SDK + Unity WebGL**
        
- 🪙 On-chain **NFT item ownership and trading**
    
- 🔐 Secure in-game wallet integration using **Phantom Connect**
    
- ⚙️ Real-time state sync with **Solana RPC**
        

---

## 🧱 Tech Stack

|Layer|Technology|
|---|---|
|**Game Engine**|Unity 6|
|**Hosting**|IDosGame|
|**Version Control**|GitHub|

---

## 🏗️ Architecture

```
 ┌────────────────────────────┐
 │         Player             │
 │    (Unity WebGL Game)      │
 └─────────────┬──────────────┘
               │
               ▼
       IDosGame SDK
               │
               ▼
   ┌──────────────────────────┐
   │   On-Chain Components    │
   │ - NFT minting system     │
   │ - Item ownership         │
   │ - Quest reward tracking  │
   └──────────────────────────┘
               │
               ▼
        Off-chain analytics
```

---

    

### 🚀 Run Locally

```bash
# 1. Clone repo
git clone https://github.com/kusetos/SolanaWebGame.git
cd SolanaWebGame

# 2. Open project in Unity
# Make sure URP and Solana Unity SDK are installed

# 3. Build WebGL version
File → Build Settings → WebGL → Build

# 4. Launch locally
npx serve ./Build
```

---

## 🔮 Long-Term Vision

We aim to create a **player-owned learning metaverse** powered by Solana:

- Cross-game identity and inventory
    
- Player-to-player marketplaces
    
- DAO-driven educational content creation
    
- Integration with other Solana dApps (guilds, collectibles, tournaments)
    
