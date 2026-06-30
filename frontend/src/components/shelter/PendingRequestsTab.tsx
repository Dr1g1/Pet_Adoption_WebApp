import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adoptionRequestApi } from '../../api/adoptionRequest.api';
import { UserProfileView } from './UserProfileView';

export function PendingRequestsTab({ shelterId }: { shelterId: string }) {
    const queryClient = useQueryClient();
    const [responseMsg, setResponseMsg] = useState<Record<string, string>>({});
    const [viewingUserId, setViewingUserId] = useState<string | null>(null);

    const { data: requests, isLoading } = useQuery({
        queryKey: ['pendingRequests', shelterId],
        queryFn: () => adoptionRequestApi.getPendingForShelter(shelterId)
    });

    const approve = useMutation({
        mutationFn: (requestId: string) =>
            adoptionRequestApi.approve(requestId, responseMsg[requestId] ?? ''),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['pendingRequests', shelterId] });
            queryClient.invalidateQueries({ queryKey: ['animals'] });
        }
    });

    const reject = useMutation({
        mutationFn: (requestId: string) =>
            adoptionRequestApi.reject(requestId, responseMsg[requestId] ?? ''),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: ['pendingRequests', shelterId] })
    });

    if (isLoading) return <p className="text-gray-500 text-sm">Učitavanje zahteva...</p>;

    if (!requests || requests.length === 0) {
        return <p className="text-gray-500 text-sm">Nema zahteva na čekanju.</p>;
    }

    return (
        <div className="space-y-4">
            {requests.map(req => (
                <div key={req.requestId} className="bg-white border border-gray-100 rounded-xl p-5 shadow-sm">
                    <div className="flex items-start justify-between mb-3">
                        <div>
                            <p className="font-semibold text-gray-900">{req.animalName}</p>
                            <p className="text-sm text-gray-500">
                                Tražilac:{' '}
                                <button
                                    onClick={() => setViewingUserId(req.userId)}
                                    className="text-orange-600 hover:text-orange-700 font-medium underline"
                                >
                                    {req.userName}
                                </button>
                            </p>
                            <p className="text-xs text-gray-400 mt-1">
                                {new Date(req.createdAt).toLocaleDateString('sr-RS')}
                            </p>
                        </div>
                        <span className="text-xs bg-yellow-100 text-yellow-700 px-2.5 py-1 rounded-full font-medium">
                            Na čekanju
                        </span>
                    </div>

                    <input
                        value={responseMsg[req.requestId] ?? ''}
                        onChange={e => setResponseMsg(m => ({ ...m, [req.requestId]: e.target.value }))}
                        placeholder="Poruka tražiocu (opciono)..."
                        className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm mb-3
                            focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none"
                    />

                    <div className="flex gap-2">
                        <button
                            onClick={() => approve.mutate(req.requestId)}
                            disabled={approve.isPending || reject.isPending}
                            className="bg-green-500 hover:bg-green-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50"
                        >
                            Prihvati
                        </button>
                        <button
                            onClick={() => reject.mutate(req.requestId)}
                            disabled={approve.isPending || reject.isPending}
                            className="bg-red-500 hover:bg-red-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50"
                        >
                            Odbij
                        </button>
                    </div>
                </div>
            ))}

            {viewingUserId && (
                <UserProfileView userId={viewingUserId} onClose={() => setViewingUserId(null)} />
            )}
        </div>
    );
}