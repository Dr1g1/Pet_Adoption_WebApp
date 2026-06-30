import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { shelterApi } from '../api/shelter.api';
import { joinRequestApi } from '../api/joinRequest.api';
import type { JoinRequestStatus } from '../types/joinRequest.types';
import { isAxiosError } from 'axios';

const statusLabel: Record<JoinRequestStatus, string> = {
    Pending: 'Na čekanju', Approved: 'Prihvaćen', Rejected: 'Odbijen'
};
const statusClass: Record<JoinRequestStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-700',
    Approved: 'bg-green-100 text-green-700',
    Rejected: 'bg-red-100 text-red-700'
};

export function JoinShelterPage() {
    const queryClient = useQueryClient();
    const [selectedShelter, setSelectedShelter] = useState<string | null>(null);
    const [message, setMessage] = useState('');
    const [errorMsg, setErrorMsg] = useState('');

    const { data: shelters, isLoading: sheltersLoading } = useQuery({
        queryKey: ['shelters'],
        queryFn: () => shelterApi.getAll()
    });

    const { data: myRequests, isLoading: requestsLoading } = useQuery({
        queryKey: ['joinRequests', 'my'],
        queryFn: () => joinRequestApi.getMyRequests()
    });

    const sendRequest = useMutation({
        mutationFn: ({ shelterId, msg }: { shelterId: string; msg: string }) =>
            joinRequestApi.create(shelterId, msg),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['joinRequests', 'my'] });
            setSelectedShelter(null);
            setMessage('');
            setErrorMsg('');
        },
        onError: (err: unknown) => {
            const msg = isAxiosError<{ message: string }>(err)
                ? err.response?.data?.message ?? 'Greška pri slanju zahteva.'
                : 'Greška pri slanju zahteva.';
            setErrorMsg(msg);
        }
    });

    const cancelRequest = useMutation({
        mutationFn: (requestId: string) => joinRequestApi.cancel(requestId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['joinRequests', 'my'] })
    });

    const hasPending = myRequests?.some(r => r.status === 'Pending');

    return (
        <div className="max-w-3xl mx-auto p-8 space-y-6">
            <div>
                <h1 className="text-2xl font-bold text-gray-900">Pridruži se azilu</h1>
                <p className="text-gray-500 mt-1">
                    Pošalji zahtev azilu u kom želiš da volontiraš. Administrator azila odlučuje o prihvatanju.
                </p>
            </div>

            {!requestsLoading && myRequests && myRequests.length > 0 && (
                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
                    <h2 className="font-semibold text-gray-900 mb-3">Moji zahtevi</h2>
                    <div className="space-y-2">
                        {myRequests.map(req => (
                            <div key={req.requestId} className="flex items-center justify-between border border-gray-100 rounded-lg p-3">
                                <div>
                                    <p className="text-sm font-medium text-gray-800">{req.shelterName}</p>
                                    <p className="text-xs text-gray-400">
                                        {new Date(req.createdAt).toLocaleDateString('sr-RS')}
                                    </p>
                                </div>
                                <div className="flex items-center gap-3">
                                    <span className={`text-xs px-2.5 py-1 rounded-full font-medium ${statusClass[req.status]}`}>
                                        {statusLabel[req.status]}
                                    </span>
                                    {req.status === 'Pending' && (
                                        <button
                                            onClick={() => cancelRequest.mutate(req.requestId)}
                                            disabled={cancelRequest.isPending}
                                            className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
                                        >
                                            Otkaži
                                        </button>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            )}

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
                <h2 className="font-semibold text-gray-900 mb-3">Dostupni azili</h2>

                {hasPending && (
                    <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg text-blue-700 text-sm">
                        Već imaš zahtev na čekanju. Sačekaj odgovor ili ga otkaži pre slanja novog.
                    </div>
                )}

                {sheltersLoading && <p className="text-gray-500 text-sm">Učitavanje azila...</p>}
                {!sheltersLoading && shelters?.length === 0 && (
                    <p className="text-gray-500 text-sm">Trenutno nema dostupnih azila.</p>
                )}

                <div className="space-y-2">
                    {shelters?.map(s => (
                        <div key={s.id} className="border border-gray-100 rounded-lg p-4">
                            <div className="flex items-start justify-between">
                                <div>
                                    <p className="font-medium text-gray-900">{s.name}</p>
                                    <p className="text-sm text-gray-500">{s.address}</p>
                                </div>
                                <button
                                    onClick={() => { setSelectedShelter(s.id); setErrorMsg(''); }}
                                    disabled={hasPending}
                                    className="text-sm bg-orange-500 hover:bg-orange-600 text-white font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    Pošalji zahtev
                                </button>
                            </div>

                            {selectedShelter === s.id && (
                                <div className="mt-3 pt-3 border-t border-gray-100">
                                    <textarea
                                        value={message}
                                        onChange={e => setMessage(e.target.value)}
                                        placeholder="Poruka azilu (zašto želiš da volontiraš)..."
                                        rows={2}
                                        className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm
                                            focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none"
                                    />
                                    {errorMsg && <p className="text-sm text-red-500 mt-1">{errorMsg}</p>}
                                    <div className="flex gap-2 mt-2">
                                        <button
                                            onClick={() => sendRequest.mutate({ shelterId: s.id, msg: message })}
                                            disabled={sendRequest.isPending}
                                            className="text-sm bg-orange-500 hover:bg-orange-600 text-white font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50"
                                        >
                                            {sendRequest.isPending ? 'Slanje...' : 'Potvrdi'}
                                        </button>
                                        <button
                                            onClick={() => { setSelectedShelter(null); setMessage(''); setErrorMsg(''); }}
                                            className="text-sm text-gray-600 hover:bg-gray-100 px-4 py-2 rounded-lg transition-colors"
                                        >
                                            Otkaži
                                        </button>
                                    </div>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}