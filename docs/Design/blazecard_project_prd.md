# Project PRD: BlazeCard Fintech Card Previewer

## 1. Project Overview
BlazeCard is a high-fidelity previewing utility designed for **HBF Health** to facilitate the transition from physical plastic cards to digital wallet passes. It serves as a bridge for developers and designers to configure, visualize, and generate code for digital membership cards.

### Core Value Proposition
- **"Burning plastic cards for the better"**: Streamlining the move to cardless fintech.
- **Live Accuracy**: Real-time rendering of Apple Wallet (.pkpass) and Google Wallet digital passes.
- **Multi-Modal Workspace**: A unified interface for form-based configuration, developer code generation, and visual drag-and-drop design.

---

## 2. Target Audience
- **Developers**: Implementing .NET Passbook or Google Wallet API integrations.
- **Product Owners**: Reviewing the visual fidelity and member experience of digital cards.
- **UX/UI Designers**: Customizing card layouts and branding elements.

---

## 3. Functional Requirements

### 3.1 Workspace Modes
The application operates in three distinct, tabbed modes managed via a left-hand control sidebar:

1.  **Form Mode**:
    *   Input fields for card metadata (Card Title, Member Name, Card Number, Expiry).
    *   Parameters mapped to both `.pkpass` (Apple) and Generic Pass (Google) schemas.
    *   "Build Card" action to trigger code generation and preview refresh.
2.  **Code Mode**:
    *   Displays implementation snippets for two primary stacks:
        *   **Apple**: C# / .NET using `dotnet-passbook`.
        *   **Android**: JSON / REST API requests for the Google Wallet API.
    *   Logic for "Build" button to generate missing request formats based on active form data.
3.  **Visual Mode**:
    *   Drag-and-drop toolbox for card customization.
    *   Options for Logo Upload, Background Color, Text Blocks, and QR Code placement.
    *   Grid-based canvas for precise element positioning.

### 3.2 Live Preview System
- **Unified Preview**: A centralized viewing area on the right side of the screen.
- **Single Card Focus**: Displays one high-fidelity card at a time.
- **Zoom Control**: A slider to inspect card details (Member ID, Barcode/QR) with precision.
- **Platform Parity**: Renders must match the layout constraints of modern iOS and Android wallet apps.

---

## 4. Design & Branding

### 4.1 Visual Identity
- **Mascot**: A friendly Quokka (HBF brand icon) playfully burning old-fashioned plastic cards with digital teal fire.
- **Banner**: A text-free, professional hero banner at the top of the workspace.
- **Theme Support**:
    *   **Light Theme**: Clinical and clean, pulling directly from the `hbf.com.au` palette (Signature Teal: `#009fae`).
    *   **Dark Theme**: A deep charcoal-based complimentary theme for low-light developer workflows.

### 4.2 UI Components
- **Sidebar**: Fixed left-hand navigation containing mode tabs and contextual tools.
- **Top Bar**: Minimalist, focusing on workspace title and account actions.
- **Canvas**: Subtle grid pattern in Visual Mode for layout alignment.

---

## 5. Technical Architecture
- **Framework**: Blazor (C#) for a rich, interactive SPA experience.
- **Deployment**: Launched in Docker containers for environment consistency.
- **Libraries**:
    *   [dotnet-passbook](https://github.com/tomasmcguinness/dotnet-passbook) for Apple Wallet generation.
    *   Google Wallet API for Android pass generation.
- **Mocking**: Use dummy certificates for preview purposes; no actual signing of production passes required in the previewer.

---

## 6. Success Metrics
- **Fidelity**: Previewed cards match the final generated passes with 99% accuracy.
- **Efficiency**: Reduced time-to-implementation for developers by providing ready-to-use code snippets.
- **Branding**: High scores in brand alignment surveys during internal HBF reviews.
