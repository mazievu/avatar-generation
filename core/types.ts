import { Language } from './localization';

export enum Gender {
  Male = 'Male',
  Female = 'Female'
}

export enum LifePhase {
  Newborn = 'Newborn', // 0-5
  Elementary = 'Elementary School', // 6-11
  MiddleSchool = 'Middle School', // 12-15
  HighSchool = 'High School', // 16-18
  University = 'University', // 19-22
  PostGraduation = 'Working Life', // 23-59
  Retired = 'Retired' // 60+
}

export enum CharacterStatus {
    Idle = 'Idle',
    InEducation = 'In Education',
    Working = 'Working',
    Unemployed = 'Unemployed',
    Internship = 'Internship',
    VocationalTraining = 'Vocational Training',
    Retired = 'Retired',
    Trainee = 'Trainee',
}

export enum RelationshipStatus {
    Single = 'Single',
    Married = 'Married',
}

export enum PetType {
    Dog = 'Dog',
    Cat = 'Cat',
    Parrot = 'Parrot',
    Horse = 'Horse',
    Fish = 'Fish/Turtle',
}

export interface Stats {
  iq: number; // 0-200
  happiness: number; // 0-100
  eq: number; // 0-100
  health: number; // 0-100
  skill: number; // 0-100
}

export interface Pet {
    id: string;
    name: string;
    type: PetType;
    ownerId: string;
    age: number;
}

export interface BusinessSlot {
    role: string;
    requiredMajor: string;
    assignedCharacterId: string | null; // Can be a family member or a Robot
}

export interface Business {
    id: string;
    name: string;
    type: string;
    level: number;
    ownerId: string;
    slots: BusinessSlot[];
    baseRevenue: number;
}

// ---------------------------------------------
// Avatar Types
// ---------------------------------------------
export type LayerKey =
  | "background"
  | "backHair"
  | "features" // freckles, blush, face marks
  | "eyes"
  | "eyebrows"
  | "mouth"
  | "beard" // male facial hair
  | "frontHair";

export interface LayerOption {
  id: string; // unique within its layer
  name: string;
  src: string; // path to transparent PNG
  previewSrc?: string; // optional lighter preview asset
  ageCategory?: 'baby' | 'normal' | 'old';
}

export interface LayerDefinition {
  key: LayerKey;
  label: string;
  zIndex: number; // draw order (low -> high)
  options: LayerOption[];
  required?: boolean; // if true, must have a selection
  allowNone?: boolean; // if true, can select "none"
}

export type Manifest = LayerDefinition[];

export type AvatarState = Partial<Record<LayerKey, string | null>> & {
  backHairColor?: string;
  frontHairColor?: string;
  eyebrowsColor?: string;
  beardColor?: string;
};


export interface Character {
  id: string;
  name: string;
  gender: Gender;
  generation: number;
  birthDate: { day: number, year: number };
  age: number;
  isAlive: boolean;
  deathDate: { day: number, year: number } | null;
  stats: Stats;
  phase: LifePhase;
  education: string;
  major: string | null;
  careerTrack: string | null;
  careerLevel: number;
  status: CharacterStatus;
  statusEndYear: number | null;
  relationshipStatus: RelationshipStatus;
  partnerId: string | null;
  childrenIds: string[];
  parentsIds: [string, string] | [];
  isPlayerCharacter: boolean;
  mourningUntilYear: number | null;
  monthlyNetIncome: number;
  eventsThisYear: number;
  petId: string | null;
  completedOneTimeEvents: string[];
  currentClubs: string[];
  completedClubEvents: string[];
  displayAdjective: { key: string; year: number; } | null;
  avatarState: AvatarState;
  staticAvatarUrl?: string;
  progressionPenalty?: number;
  traineeForCareer?: string | null;
  childrenEventCooldownUntil?: { day: number; year: number; } | null;
  lowHappinessYears: number; // New: Tracks years happiness is below threshold
  lowHealthYears: number; // New: Tracks years health is below threshold
  monthsInCurrentJobLevel: number; // New: Tracks months in current job level
}
export enum AssetType {
    Housing = 'Housing',
    Vehicle = 'Vehicle',
    Electronics = 'Electronics',
    Art = 'Art',
    Jewelry = 'Jewelry',
    Furniture = 'Furniture',
    Library = 'Library',
    Gym = 'Gym',
    Music = 'Music',
    Vacation = 'Vacation',
}

export interface AssetDefinition {
    id: string;
    type: AssetType;
    tier: 1 | 2 | 3;
    nameKey: string;
    descriptionKey: string;
    cost: number;
    effects: Partial<Stats>; // Monthly buffs
    imageSrc?: string; // Path to the asset image
}


export interface PurchasedAsset {
    id: string; // Corresponds to AssetDefinition id
    purchaseYear: number;
}


export interface CareerChoice {
    characterId: string;
    options: string[];
}

export interface GameLogEntry {
    year: number;
    messageKey: string;
    replacements?: Record<string, string | number>;
    statChanges?: Partial<Stats>;
    fundChange?: number;

    // New optional fields for detailed view
    characterId?: string;
    eventTitleKey?: string;
}

export interface Loan {
    id: string;
    amount: number;
    dueDate: { day: number; year: number };
}

export interface GameState {
  familyMembers: Record<string, Character>;
  familyFund: number;
  purchasedAssets: PurchasedAsset[];
  familyPets: Record<string, Pet>;
  familyBusinesses: Record<string, Business>;
  currentDate: { day: number, year: number };
  gameLog: GameLogEntry[];
  gameOverReason: 'victory' | 'bankruptcy' | 'debt' | null;
  activeEvent: { characterId: string, event: GameEvent, replacements?: Record<string, string | number> } | null;
  pendingSchoolChoice: { characterId: string; newPhase: LifePhase }[] | null;
  pendingUniversityChoice: { characterId: string }[] | null;
  pendingMajorChoice: { characterId: string; options: UniversityMajor[] } | null;
  pendingClubChoice: { characterId: string; options: Club[] } | null;
  pendingCareerChoice: CareerChoice | null;
  pendingUnderqualifiedChoice?: { characterId: string; careerTrackKey: string; } | null;
  pendingLoanChoice: boolean | null;
  pendingPromotion: { characterId: string; newLevel: number; newTitleKey: string } | null;
  eventQueue: { characterId: string, event: GameEvent, replacements?: Record<string, string | number> }[];
  activeLoans: Loan[];
  highestEducation: string;
  highestCareer: string;
  totalMembers: number;
  monthlyNetChange: number;
  eventCooldownUntil: { day: number, year: number } | null;
  lang: Language;
}

export type StatChanges = Partial<Stats>;
export type TriggeredEvent = { 
  eventId: string; 
  chance: number; 
  reTarget?: 'parents'; 
};

export interface DynamicEffectResult {
    statChanges?: StatChanges;
    fundChange?: number;
    logKey: string;
    triggers?: TriggeredEvent[];
}

export interface EventEffect {
  statChanges?: StatChanges;
  fundChange?: number;
  logKey: string;
  triggers?: TriggeredEvent[];
  action?: (state: GameState, charId: string) => Partial<GameState>;
  getDynamicEffect?: () => DynamicEffectResult;
}

export interface GameEvent {
  id: string;
  titleKey: string;
  descriptionKey: string;
  phases: LifePhase[];
  choices: EventChoice[];
  isTriggerOnly?: boolean;
  isMilestone?: boolean;
  allowedRelationshipStatuses?: RelationshipStatus[];
  applyEffectToAll?: boolean;
  condition?: (state: GameState, char: Character) => boolean;
}

export interface SchoolOption {
    nameKey: string;
    cost: number;
    effects: StatChanges;
    logKey: string;
}

export interface UniversityMajor {
    nameKey: string;
    descriptionKey: string;
    cost: number;
    effects: StatChanges;
}

export interface CareerTrack {
    nameKey: string;
    descriptionKey: string;
    levels: { titleKey: string; salary: number; skillRequired: number }[];
    requiredMajor?: string;
    iqRequired: number;
    eqRequired: number;
}

export interface PetDefinition {
    nameKey: string;
    monthlyCost: number;
    effects: Partial<Stats>;
}

export interface BusinessSlotDefinition {
    roleKey: string;
    requiredMajor: string;
}

// FIX: Add 'id', 'type', and 'tier' to BusinessDefinition to match its usage in constants.ts.
export interface BusinessDefinition {
    id: string;
    type: string;
    tier: number;
    nameKey: string;
    cost: number;
    baseRevenue: number; // monthly
    costOfGoodsSold: number; // percentage of revenue
    fixedMonthlyCost: number;
    slots: BusinessSlotDefinition[];
    upgradeSlots: BusinessSlotDefinition[];
}

export interface GameScenario {
  id: string;
  nameKey: string;
  descriptionKey: string;
  themeColor: string;
  createInitialState: (initialYear: number, lang: Language) => GameState;
}

export interface Club {
  id: string;
  nameKey: string;
  descriptionKey: string;
  prerequisites: { age?: number; stats?: Partial<Stats>; };
  effects: StatChanges;
  careerInfluence: string[];
  lifePhase: LifePhase[];
}

export interface ClubEvent {
  id: string;
  titleKey: string;
  descriptionKey: string;
  clubId: string;
  choices: EventChoice[];
  condition?: (state: GameState, char: Character) => boolean;
}