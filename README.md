# Seasoned

In progress link: https://seasoned.ddns.net/

The Pitch:

Seasoned is a high-performance, private digital cookbook that bridges the gap between web discovery and kitchen execution. By combining the multimodal power of Gemini 2.5 Flash with a secure, self-hosted PostgreSQL backbone, Seasoned allows users to instantly "distill" messy recipe blogs and food photos into a standardized, searchable, and shareable library they truly own.

Target Audience:

    The Modern Minimalist: Cooks who want an ad-free, recipe experience.

    The Legacy Keeper: Families digitizing handwritten recipes into a clean digital format.

    The Privacy Enthusiast: Users who want the power of AI without storing their personal data in a massive  cloud.

The Hybrid Tech Stack:

| Components | Technology |
| :--- | :--- |
| **Hosting** | Private Server (Dockerized on home hardware) |
| **CI/CD** | Jenkins server |
| **Frontend** | Nuxt 4 + Vuetify + CSS |
| **Backend** | Dotnet |
| **Database** | Postgres + pgvector |
| **Intelligence** | Gemini 2.5 Flash |
| **Storage** | Local File System |

Technical Requirements:

1. AI & Multimodal Intelligence

    Multimodal Extraction: Use Gemini 1.5 Flash to accept image/jpeg inputs and return a strictly validated JSON Schema containing title, ingredients, and steps.

    Semantic Search: Implement pgvector in the local database. Recipes will be converted into "embeddings" (via Gemini) to allow users to search for "Comfort food for a rainy day" instead of just keyword matches.

2. Full-Stack Architecture (Nuxt 4)

    Directory Structure: Adherence to the new app/ directory standard for better IDE performance and separation of concerns.

    Serverless-Style Routes: Use Nitro server routes to keep the Gemini API Key hidden from the client-side.

    Responsive Design: A UI that adapts perfectly to a tablet propped up on a kitchen counter.

3. Data & Storage

    Relational Schema: A PostgreSQL database to manage Users, Recipes, Tags, and Shares.

    Private Media Pipeline: A custom upload handler that saves images to a local Docker volume, served via a secured static asset route.

    Sharing Permissions: A relational join-table logic that allows one user to "push" a recipe to another user's library.

Use Cases:

    Photo-to-Recipe: User snaps a picture of a magazine page; Gemini extracts the text; the user saves it to their Postgres DB.

    Semantic Discovery: User searches for "High protein dinner with lime" and the app uses vector similarity to find the best match.

    Ad-Free Web Scraping: User pastes a blog URL; the server fetches the content, and Gemini strips out the ads and life stories.

    Collaborative Boxes: One user "seasons" a recipe (rates/tags it) and shares it with someone who also uses the instance.