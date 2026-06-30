import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { joinRequestApi } from '../../api/joinRequest.api';

export function JoinRequestsTab({ shelterId }: { shelterId: string }) {
    const queryClient = useQueryClient();

    const { data: requests, isLoading } = useQuery({
        queryKey: ['joinRequests', 'pending', shelterId],
        queryFn: () => joinRequestApi.getPendingForShelter(shelterId)
    });

    const approve = useMutation({
        mutationFn: (requestId: string) => joinRequestApi.approve(requestId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['joinRequests', 'pending', shelterId] });
            queryClient.invalidateQueries({ queryKey: ['volunteers', 'shelter', shelterId] });
        }
    });

    const reject = useMutation({
        mutationFn: (requestId: string) => joinRequestApi.reject(requestId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['joinRequests', 'pending', shelterId] })
    });

    if (isLoading) return <p className="text-gray-500 text-sm">Učitavanje zahteva...</p>;
    if (!requests || requests.length === 0) {
        return <p className="text-gray-500 text-sm">Nema zahteva za volontiranje na čekanju.</p>;
    }

    return (
        <div className="space-y-4">
            {requests.map(req => (
                <div key={req.requestId} className="bg-white border border-gray-100 rounded-xl p-5 shadow-sm">
                    <div className="flex items-start justify-between mb-3">
                        <div>
                            <p className="font-semibold text-gray-900">{req.volunteerName}</p>
                            <p className="text-xs text-gray-400 mt-1">
                                {new Date(req.createdAt).toLocaleDateString('sr-RS')}
                            </p>
                            {req.message && (
                                <p className="text-sm text-gray-600 mt-2 italic">"{req.message}"</p>
                            )}
                        </div>
                        <span className="text-xs bg-yellow-100 text-yellow-700 px-2.5 py-1 rounded-full font-medium">
                            Na čekanju
                        </span>
                    </div>

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
        </div>
    );
}