import { useState } from 'react';
import { useAuthStore } from '../store/authStore';
import { PendingRequestsTab } from '../components/shelter/PendingRequestsTab';
import { AnimalsManageTab } from '../components/shelter/AnimalManageTab';
import { VolunteersManageTab } from '../components/shelter/VolunteersManageTab';
import { JoinRequestsTab } from '../components/shelter/JoinRequestsTab';

type Tab = 'requests' | 'join' | 'animals' | 'volunteers';

const tabs: { key: Tab; label: string }[] = [
    { key: 'requests', label: 'Zahtevi za usvajanje' },
    { key: 'join', label: 'Zahtevi za volontiranje' },
    { key: 'animals', label: 'Životinje' },
    { key: 'volunteers', label: 'Volonteri' }
];

export function ShelterManagePage() {
    const shelterId = useAuthStore(s => s.user?.shelterId);
    const [tab, setTab] = useState<Tab>('requests');

    if (!shelterId) {
        return (
            <div className="max-w-4xl mx-auto p-8">
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-yellow-800">
                    Nisi povezan/a ni sa jednim azilom.
                </div>
            </div>
        );
    }

    return (
        <div className="max-w-4xl mx-auto p-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Upravljanje azilom</h1>

            <div className="flex gap-1 border-b border-gray-200 mb-6">
                {tabs.map(t => (
                    <button
                        key={t.key}
                        onClick={() => setTab(t.key)}
                        className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
                            tab === t.key
                                ? 'border-orange-500 text-orange-600'
                                : 'border-transparent text-gray-500 hover:text-gray-700'
                        }`}
                    >
                        {t.label}
                    </button>
                ))}
            </div>

            {tab === 'requests' && <PendingRequestsTab shelterId={shelterId} />}
            {tab === 'join' && <JoinRequestsTab shelterId={shelterId} />}
            {tab === 'animals' && <AnimalsManageTab shelterId={shelterId} />}
            {tab === 'volunteers' && <VolunteersManageTab shelterId={shelterId} />}
        </div>
    );
}